using System;
using System.Collections.Generic;
using M3uParser.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;

namespace M3uParser.Modes
{
    public class ExportMode: AppMode
    {
        private string _outputFilePath;
        private int? _channelID;

        public ExportMode(string[] args) : base(args)
        {
            this._outputFilePath = GetParamValueString("path");
            this._channelID = GetParamValueInt("channel");
        }

        public override void Run()
        {
            // get active channels from DB
            List<Channel> channelsToExport;
            using (var channelDb = new ChannelsContext())
            {
                channelsToExport = channelDb.Channels
                                            .Where(channel => this._channelID == null || channel.ID == this._channelID) // filter by specific channel if passed
                                            .Include(i => i.Links)
                                            .Include(i => i.Groups)
                                            .Where(i => i.Status == Channel.ChannelStatus.Visible)
                                            .ToList();
            }

            var outRows = new List<string>(channelsToExport.Count * 2 + 1) { "#EXTM3U" };
            foreach (var channel in channelsToExport)
            {
                outRows.Add($"#EXTINF:-1 channel-id=\"{ channel.ID }\" group-title=\"{ string.Join(",", channel.Groups.Select(g => g.GroupName)) }\",{ channel.Name }");
                outRows.Add($"{ channel.Links.Where(i => i.IsActive).First().Link }");
            }

            File.WriteAllLines(this._outputFilePath, outRows);

            Logger.Info($"Exported { channelsToExport.Count } channels");
        }
    }
}
