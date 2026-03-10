using System;
using Microsoft.Data.SqlClient;
using Dapper;

namespace BoardGamesAnalytics
{
    class Program
    {
        static string connectionString = "server=(localdb)\\mssqllocaldb;database=boardgamesdb;trusted_connection=true;";

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("аналітика");
                Console.WriteLine("1. всі сесії");
                Console.WriteLine("2. топ-3 ігри за годинами");
                Console.WriteLine("3. рейтинг учасників (хвилини)");
                Console.WriteLine("4. загальна статистика");
                Console.WriteLine("0. вихід");
                Console.Write("вибір: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        ViewAllSessions();
                        break;
                    case "2":
                        TopGames();
                        break;
                    case "3":
                        MemberRanking();
                        break;
                    case "4":
                        GeneralStats();
                        break;
                    case "0":
                        exit = true;
                        break;
                }
            }
        }

        static void ViewAllSessions()
        {
            Console.Clear();
            using (var db = new SqlConnection(connectionString))
            {
                string sql = @"
                    select 
                        g.title as game, 
                        m.fullname as member, 
                        s.date, 
                        s.durationminutes 
                    from sessions s
                    join games g on s.gameid = g.id
                    join members m on s.memberid = m.id
                    order by s.date desc";

                var sessions = db.Query(sql);
                foreach (var s in sessions)
                {
                    Console.WriteLine($"{s.game} | {s.member} | {s.date:yyyy-MM-dd} | {s.durationminutes} хв");
                }
            }
            WaitForKey();
        }

        static void TopGames()
        {
            Console.Clear();
            using (var db = new SqlConnection(connectionString))
            {
                string sql = @"
                    select top 3 
                        g.title, 
                        sum(cast(s.durationminutes as float)) / 60.0 as totalhours
                    from sessions s
                    join games g on s.gameid = g.id
                    group by g.title
                    order by totalhours desc";

                var games = db.Query(sql);
                foreach (var g in games)
                {
                    Console.WriteLine($"{g.title} - {g.totalhours:f2} год");
                }
            }
            WaitForKey();
        }

        static void MemberRanking()
        {
            Console.Clear();
            using (var db = new SqlConnection(connectionString))
            {
                string sql = @"
                    select 
                        m.fullname, 
                        sum(s.durationminutes) as totalminutes
                    from sessions s
                    join members m on s.memberid = m.id
                    group by m.fullname
                    order by totalminutes desc";

                var members = db.Query(sql);
                foreach (var m in members)
                {
                    Console.WriteLine($"{m.fullname} - {m.totalminutes} хв");
                }
            }
            WaitForKey();
        }

        static void GeneralStats()
        {
            Console.Clear();
            
            Console.WriteLine("1. за весь час");
            Console.WriteLine("2. за період");
            Console.Write("вибір: ");
            var choice = Console.ReadLine();

            using (var db = new SqlConnection(connectionString))
            {
                string sql;
                object? param = null;

                if (choice == "2")
                {
                    Console.Write("від (yyyy-MM-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime fromDate)) return;
                    
                    Console.Write("до (yyyy-MM-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime toDate)) return;

                    sql = @"
                        select 
                            count(id) as sessioncount, 
                            coalesce(sum(durationminutes), 0) as totalduration
                        from sessions
                        where date >= @from and date <= @to";
                    param = new { from = fromDate, to = toDate };
                }
                else
                {
                    sql = @"
                        select 
                            count(id) as sessioncount, 
                            coalesce(sum(durationminutes), 0) as totalduration
                        from sessions";
                }

                var stats = db.QuerySingle(sql, param);
                Console.WriteLine($"сесій: {stats.sessioncount}");
                Console.WriteLine($"загальна тривалість: {stats.totalduration} хв ({stats.totalduration / 60.0f:f2} год)");
            }
            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.ReadKey();
        }
    }
}
