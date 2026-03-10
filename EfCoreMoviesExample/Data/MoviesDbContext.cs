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
            // Для прикладу використовуємо LocalDB. За необхідності можна змінити на іншу БД (наприклад, SQLite чи PostgreSQL)
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MoviesDbEfCore;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
