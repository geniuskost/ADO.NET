using EfCoreMoviesExample.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreMoviesExample.Data
{
    public class MoviesDbContext : DbContext
    {
        public DbSet<Title> Titles { get; set; }
        public DbSet<User> Users { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=moviesintrodb;trusted_connection=true;trustservercertificate=true;");
        }
    }
}
