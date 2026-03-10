using System;
using BoardGamesManagement.Data;

namespace BoardGamesManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("створення бази даних...");
            using (var db = new AppDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Console.WriteLine("успіх.");
            }
        }
    }
}
