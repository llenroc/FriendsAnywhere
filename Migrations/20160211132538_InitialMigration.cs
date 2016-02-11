using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace FriendsAnywhere.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friend",
                columns: table => new
                {
                    FriendId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelSid = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    UserSid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => x.FriendId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Friend");
        }
    }
}
