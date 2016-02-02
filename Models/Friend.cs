using System.ComponentModel.DataAnnotations;
using Microsoft.Data.Entity;
 
namespace ipm_quickstart_csharp_mac.Models
{
    public class FriendsContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Declare that we want to use SQLite and name the database
			optionsBuilder.UseSqlite("Data Source=./friends.db");
        }
    }
    public class Friend
    {
        [Key]
        public int FriendId { get; set; }
        [Display(Name = "Friend Name")]
        public string Name { get; set; }
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
        [ScaffoldColumn(false)] 
        public string ChannelSid { get; set; }
        [ScaffoldColumn(false)] 
        public string UserSid { get; set; }
    }
}