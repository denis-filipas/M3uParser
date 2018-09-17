using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M3uParser.DatabaseModel
{
    public enum ChannelGroupsEnum
    {
        [Description("Россия")]
        Russian,

        [Description("Украина")]
        Ukrainian,
        HD,
        Science,
        Sport,
        Films,
        Child
    }

    public class ChannelGroup
    {
        
        public int ChannelID { get; set; }

        public string GroupName { get; set; }

        protected ChannelGroup()
        { }
        private ChannelGroup(ChannelGroupsEnum @enum)
        {
            this.GroupName = @enum.ToString();
        }

        public static implicit operator ChannelGroup(ChannelGroupsEnum @enum) => new ChannelGroup(@enum);

        public static implicit operator ChannelGroupsEnum(ChannelGroup groups) => (ChannelGroupsEnum)Enum.Parse(typeof(ChannelGroupsEnum), groups.GroupName);
    }

    public static class ChannelGroupsExtensions
    {
        public static string GetDescription(this ChannelGroupsEnum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : val.ToString();
        }
    }
}
