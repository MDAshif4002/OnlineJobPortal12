using Microsoft.EntityFrameworkCore;

namespace OnlineJobPortal.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<adminlogin> adminlogin { get; set; }

        public DbSet<slider> slider { get; set; }

        public DbSet<register> register { get; set; }

        public DbSet<category> category { get; set; }

        public DbSet<product> product { get; set; }

        public DbSet<singleorder> singleorder { get; set; }
        public DbSet<cart> cart { get; set; }
        public DbSet<checkout> checkout { get; set; }


    }
}
