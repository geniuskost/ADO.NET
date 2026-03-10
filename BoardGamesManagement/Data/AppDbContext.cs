using System;
using System.Collections.Generic;
using BoardGamesManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGamesManagement.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=boardgamesdb;trusted_connection=true;trustservercertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint("ck_game_minplayers", "minplayers > 0"));
                entity.ToTable(t => t.HasCheckConstraint("ck_game_maxplayers", "maxplayers > 0"));
            });

            modelBuilder.Entity<Member>()
                .Property(m => m.JoinDate)
                .HasColumnType("datetime");

            modelBuilder.Entity<Session>()
                .Property(s => s.Date)
                .HasColumnType("datetime");

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Game)
                .WithMany(g => g.Sessions)
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Member)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            var games = new List<Game>
            {
                new Game { Id = 1, Title = "catan", Genre = "strategy", MinPlayers = 3, MaxPlayers = 4 },
                new Game { Id = 2, Title = "carcassonne", Genre = "strategy", MinPlayers = 2, MaxPlayers = 5 },
                new Game { Id = 3, Title = "dixit", Genre = "party", MinPlayers = 3, MaxPlayers = 6 },
                new Game { Id = 4, Title = "ticket to ride", Genre = "family", MinPlayers = 2, MaxPlayers = 5 },
                new Game { Id = 5, Title = "monopoly", Genre = "economic", MinPlayers = 2, MaxPlayers = 8 }
            };
            modelBuilder.Entity<Game>().HasData(games);

            var members = new List<Member>
            {
                new Member { Id = 1, FullName = "іван іванов", JoinDate = new DateTime(2023, 1, 15) },
                new Member { Id = 2, FullName = "петро петров", JoinDate = new DateTime(2023, 3, 22) },
                new Member { Id = 3, FullName = "олена оленівна", JoinDate = new DateTime(2023, 5, 10) },
                new Member { Id = 4, FullName = "марія маріївна", JoinDate = new DateTime(2023, 7, 5) },
                new Member { Id = 5, FullName = "дмитро дмитров", JoinDate = new DateTime(2023, 9, 30) }
            };
            modelBuilder.Entity<Member>().HasData(members);

            var sessions = new List<Session>();
            var random = new Random(12345);
            int sessionId = 1;

            for (int i = 0; i < 20; i++)
            {
                int gameId = random.Next(1, 6);
                int memberId = random.Next(1, 6);
                int daysAgo = random.Next(1, 365);
                int duration = random.Next(30, 240);

                sessions.Add(new Session
                {
                    Id = sessionId++,
                    GameId = gameId,
                    MemberId = memberId,
                    Date = DateTime.Now.AddDays(-daysAgo),
                    DurationMinutes = duration
                });
            }
            modelBuilder.Entity<Session>().HasData(sessions);
        }
    }
}
