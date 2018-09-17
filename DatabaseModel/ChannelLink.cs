using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace M3uParser.DatabaseModel
{
    public class ChannelLink
    {
        public int ChannelID { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }

        public DateTime? LastCheckDate { get; set; }

        public override string ToString()
        {
            return $"{ ChannelID }: { Link }";
        }
    }


    public class ChannelLinkComparer : IEqualityComparer<ChannelLink>
    {
        public bool Equals(ChannelLink x, ChannelLink y)
        {
            return x.Link.Equals(y.Link, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ChannelLink obj)
        {
            return obj.Link.ToLowerInvariant().GetHashCode();
        }
    }
}
