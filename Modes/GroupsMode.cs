using System;
using System.Linq;
using M3uParser.DatabaseModel;
using Microsoft.EntityFrameworkCore;

namespace M3uParser.Modes
{
    public class GroupsMode: AppMode
    {
        public GroupsMode(string[] args) : base(args)
        {
        }

        public override void Run()
        {
            using (var channelsDB = new ChannelsContext())
            {
                channelsDB.Channels
                          .Include(i => i.Groups)
                          .AsParallel()
                          .ForAll(channel => AssignGroups(ref channel));
                
                var count = channelsDB.SaveChanges();

                Logger.Info($"Updated { count } rows");
            }
        }

        private void AssignGroups(ref Channel channel)
        {
            if (channel.Name.Contains("HD") && !channel.Groups.Contains(ChannelGroupsEnum.HD))
                channel.Groups.Add(ChannelGroupsEnum.HD);

            if (channel.Name.ToLower().Contains("росс") && !channel.Groups.Contains(ChannelGroupsEnum.Russian))
                channel.Groups.Add(ChannelGroupsEnum.Russian);
        }
    }
}
