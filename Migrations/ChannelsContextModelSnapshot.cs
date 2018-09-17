﻿// <auto-generated />
using System;
using M3uParser.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace M3uParser.Migrations
{
    [DbContext(typeof(ChannelsContext))]
    partial class ChannelsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("M3uParser.DatabaseModel.Channel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelAlias", b =>
                {
                    b.Property<string>("Alias")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelID");

                    b.HasKey("Alias");

                    b.HasIndex("ChannelID");

                    b.ToTable("ChannelAliases");
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelGroup", b =>
                {
                    b.Property<int>("ChannelID");

                    b.Property<string>("GroupName");

                    b.HasKey("ChannelID", "GroupName");

                    b.ToTable("ChannelGroups");
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelLink", b =>
                {
                    b.Property<int>("ChannelID");

                    b.Property<string>("Link");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsLocked");

                    b.Property<DateTime?>("LastCheckDate");

                    b.HasKey("ChannelID", "Link");

                    b.ToTable("ChannelLinks");
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelAlias", b =>
                {
                    b.HasOne("M3uParser.DatabaseModel.Channel")
                        .WithMany("Aliases")
                        .HasForeignKey("ChannelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelGroup", b =>
                {
                    b.HasOne("M3uParser.DatabaseModel.Channel")
                        .WithMany("Groups")
                        .HasForeignKey("ChannelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("M3uParser.DatabaseModel.ChannelLink", b =>
                {
                    b.HasOne("M3uParser.DatabaseModel.Channel")
                        .WithMany("Links")
                        .HasForeignKey("ChannelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}