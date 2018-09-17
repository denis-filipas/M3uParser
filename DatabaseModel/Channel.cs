using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M3uParser.DatabaseModel
{
    public class Channel
    {
        public enum ChannelStatus
        { 
            Unsorted = 0, // initial status when added
            Hidden = -1, // just not sorted yet
            Ignored = -2, // never shown
            Visible = 1, // currently active
            NoActiveLinks = 2, // active, but have no links right now
            NotApproved = 3 // have active links but not active 
        }

        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; } 
        public List<ChannelLink> Links { get; set; }
        public List<ChannelGroup> Groups { get; set; }
        public List<ChannelAlias> Aliases { get; set; }

        public int Order { get; set; }
        public ChannelStatus Status { get; set; }

        public override string ToString()
        {
            return $"{ ID }: { Name }";
        }
    }
}
