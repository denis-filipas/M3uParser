using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace M3uParser.DatabaseModel
{
    public class ChannelsContext : DbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelGroup> ChannelGroups { get; set; }
        public DbSet<ChannelLink> ChannelLinks { get; set; }
        public DbSet<ChannelAlias> ChannelAliases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=channels.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ChannelGroup>().HasKey(table => new
            {
                table.ChannelID,
                table.GroupName
            });

            builder.Entity<ChannelLink>().HasKey(table => new
            {
                table.ChannelID,
                table.Link
            });

            builder.Entity<Channel>()
                   .Property(i => i.Status)
                   .HasConversion(new EnumToStringConverter<Channel.ChannelStatus>());
        }
    }

    //public static class ChannelsContextExtenstions
    //{
    //    public static void SeedEnumValues<T, TEnum>(this DbSet<T> dbSet, Func<TEnum, T> converter) where T : class
    //    {
    //        Enum.GetValues(typeof(TEnum))
    //                   .Cast<object>()
    //                   .Select(value => converter((TEnum)value))
    //                   .ToList()
    //                   .ForEach(instance => dbSet.Add(instance));
    //    }
    //}
}
