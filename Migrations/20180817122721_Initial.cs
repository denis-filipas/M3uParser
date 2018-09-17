using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace M3uParser.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChannelAliases",
                columns: table => new
                {
                    ChannelID = table.Column<int>(nullable: false),
                    Alias = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelAliases", x => x.Alias);
                    table.ForeignKey(
                        name: "FK_ChannelAliases_Channels_ChannelID",
                        column: x => x.ChannelID,
                        principalTable: "Channels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelGroups",
                columns: table => new
                {
                    ChannelID = table.Column<int>(nullable: false),
                    GroupName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelGroups", x => new { x.ChannelID, x.GroupName });
                    table.ForeignKey(
                        name: "FK_ChannelGroups_Channels_ChannelID",
                        column: x => x.ChannelID,
                        principalTable: "Channels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelLinks",
                columns: table => new
                {
                    ChannelID = table.Column<int>(nullable: false),
                    Link = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    LastCheckDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelLinks", x => new { x.ChannelID, x.Link });
                    table.ForeignKey(
                        name: "FK_ChannelLinks_Channels_ChannelID",
                        column: x => x.ChannelID,
                        principalTable: "Channels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelAliases_ChannelID",
                table: "ChannelAliases",
                column: "ChannelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelAliases");

            migrationBuilder.DropTable(
                name: "ChannelGroups");

            migrationBuilder.DropTable(
                name: "ChannelLinks");

            migrationBuilder.DropTable(
                name: "Channels");
        }
    }
}
