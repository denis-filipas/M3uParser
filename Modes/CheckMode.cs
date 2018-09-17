using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using M3uParser.DatabaseModel;
using Microsoft.EntityFrameworkCore;

namespace M3uParser.Modes
{
    public class CheckMode: AppMode
    {
        private int? _channelID;
        private Channel.ChannelStatus? _channelStatus;

        public CheckMode(string[] args) : base(args)
        {
            this._channelID = GetParamValueInt("channel");
            this._channelStatus = GetParamValueInt("status").HasValue ? (Channel.ChannelStatus?)GetParamValueInt("status").Value : null;
        }

        public override void Run()
        {
            using(var channelsDB = new ChannelsContext())
            {
                int counter = 0;

                var tasks = channelsDB.Channels
                          .Include(i => i.Links)
                          .Where(channel =>
                                 (this._channelID == null || channel.ID == this._channelID) // filter by specific channel if passed
                                 && (this._channelStatus == null || channel.Status == this._channelStatus.Value) // filter by specific channel status if passed
                            )
                          .SelectMany(channel => channel.Links)
                          .Where(link => !link.IsLocked)
                          .AsParallel()
                          .Select(async link =>
                          {
                              //var task = CheckLink(link.Link);
                              //link.IsActive = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(30).Milliseconds)) == task ? task.Result : false;
                              link.IsActive = await CheckLink(link.Link);
                              link.LastCheckDate = DateTime.Now;

                              Logger.Info($"{ counter++ }:[{ (link.IsActive ? "Active" : "Inactive") }] { link.Link }");
                          })
                          .ToList();

                Task.WhenAll(tasks).Wait();

                channelsDB.SaveChanges();

                var totalCount = channelsDB.ChannelLinks.Count();
                var activeCount = channelsDB.ChannelLinks.Where(i => i.IsActive).Count();
                Logger.Info($"Total links: { totalCount }; active: { activeCount }; inactive: { totalCount - activeCount }");
            }

            CheckActiveChannels();
        }

        private async Task<bool> CheckLink(string link)
        {
            try
            {
                var request = WebRequest.Create(link);
                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = response.GetResponseStream();
                        var buffer = new byte[1024];

                        if (stream.ReadAsync(buffer, 0, 1024).Wait(TimeSpan.FromSeconds(10).Milliseconds))
                            return !buffer.All(i => i == 0);
                        else
                            return false;
                    }
                }
            }
            catch { }

            return false;
        }

        private void CheckActiveChannels()
        {
            using (var channelsDB = new ChannelsContext())
            {
                //var channelsToHide = channelsDB.Channels
                //          .Include(c => c.Links)
                //          .Where(c => c.Status == Channel.ChannelStatus.Visible && !c.Links.Any(l => l.IsActive))
                //          .ToList();

                //foreach (var channel in channelsToHide)
                //{
                //    Logger.Info($"{ channel.ToString() } has no active links, set to NoActiveLinks");
                //    channel.Status = Channel.ChannelStatus.NoActiveLinks;
                //}
                //channelsDB.SaveChanges();

                //var channelsToShow = channelsDB.Channels
                //        .Include(c => c.Links)
                //        .Where(c => c.Status == Channel.ChannelStatus.NoActiveLinks && c.Links.Any(l => l.IsActive))
                //        .ToList();

                //foreach (var channel in channelsToShow)
                //{
                //    Logger.Info($"{ channel.ToString() } has active links, set to Visible");
                //    channel.Status = Channel.ChannelStatus.Visible;
                //}
                //channelsDB.SaveChanges();

                //var hiddenChannels = channelsDB.Channels
                //                               .Include(c => c.Links)
                //                               .Where(c => (c.Status != Channel.ChannelStatus.Visible && c.Status != Channel.ChannelStatus.Ignored) && c.Links.Any(l => l.IsActive))
                //                               .ToList();

                //foreach (var channel in hiddenChannels)
                //{
                //    Logger.Info($"{ channel.ToString() } has active links, but hidden");
                //    channel.Status = Channel.ChannelStatus.NotApproved;
                //}
                //channelsDB.SaveChanges();

                var channels = channelsDB.Channels.Include(c => c.Links).ToList();
                foreach (var channel in channels)
                {
                    var hasActiveLinks = channel.Links.Any(l => l.IsActive);

                    // hide active channels that has no active links
                    if (channel.Status == Channel.ChannelStatus.Visible && !hasActiveLinks)
                    {
                        Logger.Info($"{ channel.ToString() } has no active links, set to NoActiveLinks");
                        channel.Status = Channel.ChannelStatus.NoActiveLinks;
                    }

                    // hide pending channels that has no active links
                    else if (channel.Status == Channel.ChannelStatus.NotApproved && !hasActiveLinks)
                    {
                        Logger.Info($"{ channel.ToString() } has no active links, set to Hidden");
                        channel.Status = Channel.ChannelStatus.Hidden;
                    }

                    // show active channel if has links
                    else if (channel.Status == Channel.ChannelStatus.NoActiveLinks && hasActiveLinks)
                    {
                        Logger.Info($"{ channel.ToString() } has active links, set to Visible");
                        channel.Status = Channel.ChannelStatus.Visible;
                    }

                    // set to pending if is hidden and has active links
                    else if (channel.Status == Channel.ChannelStatus.Hidden && hasActiveLinks)
                    {
                        Logger.Info($"{ channel.ToString() } has active links, but hidden");
                        channel.Status = Channel.ChannelStatus.NotApproved;
                    }
                }

                channelsDB.SaveChanges();
            }
        }
    }
}
