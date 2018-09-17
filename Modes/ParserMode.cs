using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using M3uParser.DatabaseModel;
using M3uParser.DataModels;
using Microsoft.EntityFrameworkCore;

namespace M3uParser.Modes
{
    public class ParserMode: AppMode
    {
        private string _sourceFolderPath;
        public ParserMode(string[] args) : base(args)
        {
            this._sourceFolderPath = base.GetParamValueString("path");
        }

        public override void Run()
        {
            var sourcePlaylistFiles = Directory.GetFiles(this._sourceFolderPath);

            var allParsedChannels = sourcePlaylistFiles
                .AsParallel()
                .SelectMany(file => ParseFile(file))
                .Distinct(new SourceChannelComparer())
                .ToList();

            // group channels by name
            var cleanChannels = allParsedChannels
                .GroupBy(channel => channel.Name, new ChannelNameComparer())
                .Select(source => new Channel()
                {
                    Name = source.First().Name,
                    Aliases = new List<ChannelAlias>() { new ChannelAlias() { Alias = source.First().Name.ToLower().Trim() } },
                    Links = source.Select(i => new ChannelLink() { Link = i.Link }).Distinct(new ChannelLinkComparer()).ToList()
                })
                .ToList();

            using (var channelsDB = new ChannelsContext())
            {
                // add new channels to DB
                #region new channels

                var newChannels = cleanChannels
                    .GroupJoin(channelsDB.ChannelAliases.Select(a => a).ToList(), c => c.Name, a => a.Alias, (c, n) => new { Parsed = c, Existing = n }, new ChannelNameComparer())
                    .Where(c => !c.Existing.Any())
                    .Select(c => c.Parsed)
                    .ToList();

                if (newChannels.Any())
                {
                    channelsDB.Channels.AddRange(newChannels);
                    channelsDB.SaveChanges();
                    Logger.Info($"{ newChannels.Count } new channels added");
                }

                #endregion

                // update links for existing channels
                #region update links

                var existingChannels = channelsDB.ChannelAliases.Select(a => a).ToList();
                var channelsToUpdate = cleanChannels.Join(existingChannels, clean => clean.Name, existing => existing.Alias, (clean, existing) =>
                {
                    clean.ID = existing.ChannelID;
                    return clean;
                }).ToList();

                var pairs = channelsDB.Channels
                                      .Include(c => c.Links)
                                      .Join(channelsToUpdate, c => c.ID, parsed => parsed.ID, (existing, parsed) => new { Existing = existing, Parsed = parsed })
                                      .ToList();

                pairs.ForEach(p =>
                {
                    var newLinks = p.Parsed.Links.Except(p.Existing.Links, new ChannelLinkComparer());
                    if (newLinks.Any())
                        p.Existing.Links.AddRange(newLinks);
                });
                var newLinksAdded = channelsDB.SaveChanges();
                Logger.Info($"{ newLinksAdded } new links added");

                #endregion
            }
        }

        private List<SourceChannel> ParseFile(string filePath)
        {
            var channelsList = new List<SourceChannel>();

            try
            {
                var rawFileContentByLines = CleanupSourceFile(File.ReadAllLines(filePath));


                for (int i = 0; i < rawFileContentByLines.Length; i++)
                {
                    var line = rawFileContentByLines[i];
                    if (!line.StartsWith("#EXTINF", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var nextLine = rawFileContentByLines.Length > i + 1 ? rawFileContentByLines[i + 1] : null;
                    if (string.IsNullOrEmpty(nextLine) || !nextLine.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var channel = new SourceChannel()
                    {
                        
                        Name = line.Split(',')[1].Trim(),
                        Link = nextLine.Trim()
                    };

                    channelsList.Add(channel);
                    i++;
                }
            }
            catch (Exception ex) { Logger.Exception(ex); }

            Logger.Info($"parsed { channelsList.Count } channels in { Path.GetFileName(filePath) }");

            return channelsList;
        }

        private string[] CleanupSourceFile(string[] sourceLines)
        {
            return sourceLines
                .Select(line => line
                            .Replace("—--- http://podkola.net —---", "")
                            .Replace("—--- podkola.net —---", "")
                            .Replace("?iptvlist.net", "")
                            .Replace("iptvlist.net", "")
                            .Replace("[PremiumSlyNet]", "")
                            .Trim()
                       )
                .Where(line =>
                       line.StartsWith("#EXTINF", StringComparison.InvariantCultureIgnoreCase)
                       || line.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                      )
                .ToArray();
        }
    }
}
