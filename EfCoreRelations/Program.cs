using System;
using System.Linq;
using EfCoreRelations.Data;
using EfCoreRelations.Models;

namespace EfCoreRelations
{
    class Program
    {
        static User? currentUser = null;

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("movies app (relations)");
                if (currentUser == null)
                {
                    Console.WriteLine("1. реєстрація");
                    Console.WriteLine("2. авторизація");
                }
                else
                {
                    Console.WriteLine($"авторизовано: {currentUser.Username}");
                    Console.WriteLine("3. додати фільм");
                    Console.WriteLine("4. видалити свій фільм");
                    Console.WriteLine("5. редагувати профіль");
                    Console.WriteLine("6. вихід з акаунту");
                }
                Console.WriteLine("0. вихід");
                Console.Write("вибір: ");

                var input = Console.ReadLine();
                
                if (currentUser == null)
                {
                    switch (input)
                    {
                        case "1":
                            RegisterUser();
                            break;
                        case "2":
                            LoginUser();
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (input)
                    {
                        case "3":
                            AddMovie();
                            break;
                        case "4":
                            DeleteMovie();
                            break;
                        case "5":
                            EditProfile();
                            break;
                        case "6":
                            currentUser = null;
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("реєстрація");
            
            Console.Write("username: ");
            string username = Console.ReadLine() ?? string.Empty;

            Console.Write("email: ");
            string email = Console.ReadLine() ?? string.Empty;

            Console.Write("пароль: ");
            string password = Console.ReadLine() ?? string.Empty;

            using (var db = new RelationsDbContext())
            {
                var newUser = new User 
                { 
                    Username = username, 
                    Email = email, 
                    Password = password 
                };
                
                db.Users.Add(newUser);
                db.SaveChanges();
                Console.WriteLine("успіх.");
            }

            WaitForKey();
        }

        static void LoginUser()
        {
            Console.Clear();
            Console.WriteLine("авторизація");

            Console.Write("username: ");
            string username = Console.ReadLine() ?? string.Empty;

            Console.Write("пароль: ");
            string password = Console.ReadLine() ?? string.Empty;

            using (var db = new RelationsDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    currentUser = user;
                    Console.WriteLine("успіх.");
                }
                else
                {
                    Console.WriteLine("помилка.");
                }
            }
            WaitForKey();
        }

        static void AddMovie()
        {
            if (currentUser == null) return;

            Console.Clear();
            Console.WriteLine("додавання фільму");

            Console.Write("назва: ");
            string title = Console.ReadLine() ?? string.Empty;

            Console.Write("рік виходу: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("помилка.");
                WaitForKey();
                return;
            }

            Console.Write("опис: ");
            string description = Console.ReadLine() ?? string.Empty;

            using (var db = new RelationsDbContext())
            {
                var movie = new Movie
                {
                    Title = title,
                    ReleaseYear = year,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description,
                    UserId = currentUser.Id
                };

                db.Movies.Add(movie);
                db.SaveChanges();
                Console.WriteLine("успіх.");
            }
            WaitForKey();
        }

        static void DeleteMovie()
        {
            if (currentUser == null) return;

            Console.Clear();
            Console.WriteLine("ваші фільми:");

            using (var db = new RelationsDbContext())
            {
                var myMovies = db.Movies.Where(m => m.UserId == currentUser.Id).ToList();
                if (myMovies.Count == 0)
                {
                    Console.WriteLine("пусто.");
                    WaitForKey();
                    return;
                }

                foreach (var m in myMovies)
                {
                    Console.WriteLine($"{m.Id} - {m.Title}");
                }

                Console.Write("id фільму: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var movieToDelete = myMovies.FirstOrDefault(m => m.Id == id);
                    if (movieToDelete != null)
                    {
                        db.Movies.Remove(movieToDelete);
                        db.SaveChanges();
                        Console.WriteLine("видалено.");
                    }
                    else
                    {
                        Console.WriteLine("помилка.");
                    }
                }
            }
            WaitForKey();
        }

        static void EditProfile()
        {
            if (currentUser == null) return;

            Console.Clear();
            Console.WriteLine("редагування профілю");

            using (var db = new RelationsDbContext())
            {
                var user = db.Users.Find(currentUser.Id);
                if (user == null) return;

                Console.Write("новий username (залиште пустим щоб не змінювати): ");
                string newUsername = Console.ReadLine() ?? string.Empty;

                Console.Write("новий email (залиште пустим щоб не змінювати): ");
                string newEmail = Console.ReadLine() ?? string.Empty;

                Console.Write("новий пароль: ");
                string newPassword = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(newUsername)) user.Username = newUsername;
                if (!string.IsNullOrWhiteSpace(newEmail)) user.Email = newEmail;
                if (!string.IsNullOrWhiteSpace(newPassword)) user.Password = newPassword;

                db.SaveChanges();
                currentUser = user;
                Console.WriteLine("успіх.");
            }
            WaitForKey();
        }

        static void WaitForKey()
        {
            Console.ReadKey();
        }
    }
}
