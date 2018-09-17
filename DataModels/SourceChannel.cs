using System;
using System.Collections.Generic;
using M3uParser.DatabaseModel;

namespace M3uParser.DataModels
{
    public class SourceChannel
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    public class SourceChannelComparer : IEqualityComparer<SourceChannel>
    {
        public bool Equals(SourceChannel x, SourceChannel y)
        {
            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase)
                    && x.Link.Equals(y.Link, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(SourceChannel obj)
        {
            return (obj.Name.ToLowerInvariant() + obj.Link.ToLowerInvariant()).GetHashCode();
        }
    }

    public class ChannelNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLowerInvariant().GetHashCode();
        }
    }
}
