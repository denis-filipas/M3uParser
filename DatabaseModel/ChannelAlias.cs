using System;
using System.ComponentModel.DataAnnotations;

namespace M3uParser.DatabaseModel
{
    public class ChannelAlias
    {
        public int ChannelID { get; set; }

        [Key]
        public string Alias { get; set; }
    }
}
