using EfCoreRelations.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreRelations.Data
{
    public class RelationsDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=moviesrelationsdb;trusted_connection=true;trustservercertificate=true;");
        }
    }
}
