using System;
using System.Linq;
using EfCoreMoviesExample.Data;
using EfCoreMoviesExample.Models;

namespace EfCoreMoviesExample
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("movies app (intro)");
                Console.WriteLine("1. реєстрація користувача");
                Console.WriteLine("2. перегляд наявних юзерів");
                Console.WriteLine("0. вихід з застосунка");
                Console.Write("оберіть опцію: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        RegisterUserMenu();
                        break;
                    case "2":
                        ViewUsersMenu();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        static void RegisterUserMenu()
        {
            Console.Clear();
            Console.WriteLine("реєстрація");
            
            Console.Write("ім'я: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("логін: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("пароль: ");
            string password = Console.ReadLine() ?? string.Empty;

            try
            {
                using (var db = new MoviesDbContext())
                {
                    var newUser = new User 
                    { 
                        Name = name, 
                        Login = login, 
                        Password = password 
                    };
                    
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
                Console.WriteLine("успіх.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            WaitForKey();
        }

        static void ViewUsersMenu()
        {
            Console.Clear();
            Console.WriteLine("юзери:");

            try
            {
                using (var db = new MoviesDbContext())
                {
                    var users = db.Users.ToList();

                    if (users.Count == 0)
                    {
                        Console.WriteLine("пусто.");
                    }
                    else
                    {
                        foreach (var user in users)
                        {
                            Console.WriteLine($"{user.Id} | {user.Name} | {user.Login}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.ReadKey();
        }
    }
}
