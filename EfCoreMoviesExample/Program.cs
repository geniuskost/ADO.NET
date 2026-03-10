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
            // У реальному додатку можна було б викликати db.Database.EnsureCreated(); для автоматичного створення бази

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Головне Меню (Movies App) ===");
                Console.WriteLine("1. Реєстрація користувача");
                Console.WriteLine("2. Перегляд наявних юзерів");
                Console.WriteLine("0. Вихід з застосунка");
                Console.Write("Оберіть опцію: ");

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
                        Console.WriteLine("Невідома опція, спробуйте ще раз.");
                        WaitForKey();
                        break;
                }
            }
        }

        static void RegisterUserMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Реєстрація користувача ===");
            
            Console.Write("Введіть ім'я: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("Введіть логін: ");
            string login = Console.ReadLine() ?? string.Empty;

            Console.Write("Введіть пароль: ");
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
                    db.SaveChanges(); // Збереження нового користувача у БД
                }

                Console.WriteLine("\nКористувача успішно зареєстровано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nВиникла помилка при реєстрації (можливо, не створена БД): {ex.Message}");
            }

            WaitForKey();
        }

        static void ViewUsersMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Перегляд наявних юзерів ===");

            try
            {
                using (var db = new MoviesDbContext())
                {
                    var users = db.Users.ToList();

                    if (users.Count == 0)
                    {
                        Console.WriteLine("В базі немає зареєстрованих користувачів.");
                    }
                    else
                    {
                        foreach (var user in users)
                        {
                            Console.WriteLine($"ID: {user.Id,-3} | Ім'я: {user.Name,-15} | Логін: {user.Login}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nВиникла помилка при зчитуванні користувачів: {ex.Message}");
            }

            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення до попереднього меню...");
            Console.ReadKey();
        }
    }
}
