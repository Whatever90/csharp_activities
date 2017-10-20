using Microsoft.EntityFrameworkCore;
 
namespace connectingToDBTESTING.Models
{
    public class ActivitiesContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ActivitiesContext(DbContextOptions<ActivitiesContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Guest> Guests { get; set; }
    }
}