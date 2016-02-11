using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using ipm_quickstart_csharp_mac.Models;

namespace FriendsAnywhere.Migrations
{
    [DbContext(typeof(FriendsContext))]
    [Migration("20160211132538_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("ipm_quickstart_csharp_mac.Models.Friend", b =>
                {
                    b.Property<int>("FriendId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChannelSid");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("UserSid");

                    b.HasKey("FriendId");
                });
        }
    }
}
