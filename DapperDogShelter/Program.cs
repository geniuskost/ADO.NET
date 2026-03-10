using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using DapperDogShelter.Models;

namespace DapperDogShelter
{
    class Program
    {
        static string connectionString = "server=(localdb)\\mssqllocaldb;database=dogshelterdb;trusted_connection=true;";

        static void Main(string[] args)
        {
            InitializeDatabase();

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("dog shelter (dapper)");
                Console.WriteLine("1. додати собаку");
                Console.WriteLine("2. перегляд всіх собак");
                Console.WriteLine("3. перегляд собак в притулку");
                Console.WriteLine("4. перегляд забраних собак");
                Console.WriteLine("5. пошук собаки");
                Console.WriteLine("6. оновлення даних собаки");
                Console.WriteLine("0. вихід");
                Console.Write("вибір: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        AddDog();
                        break;
                    case "2":
                        ViewDogs("select id, name, age, breed, isadopted from dogs");
                        break;
                    case "3":
                        ViewDogs("select id, name, age, breed, isadopted from dogs where isadopted = 0");
                        break;
                    case "4":
                        ViewDogs("select id, name, age, breed, isadopted from dogs where isadopted = 1");
                        break;
                    case "5":
                        SearchDog();
                        break;
                    case "6":
                        UpdateDog();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
        }

        static void InitializeDatabase()
        {
            using (var db = new SqlConnection("server=(localdb)\\mssqllocaldb;database=master;trusted_connection=true;"))
            {
                db.Execute(@"
                    if db_id('dogshelterdb') is null
                        create database dogshelterdb;
                ");
            }

            using (var db = new SqlConnection(connectionString))
            {
                db.Execute(@"
                    if not exists (select * from sysobjects where name='dogs' and xtype='u')
                    create table dogs (
                        id int identity(1,1) primary key,
                        name nvarchar(100) not null,
                        age int not null,
                        breed nvarchar(100) not null,
                        isadopted bit not null default 0
                    )
                ");
            }
        }

        static void AddDog()
        {
            Console.Clear();
            Console.WriteLine("додавання");

            Console.Write("клічка: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("вік: ");
            if (!int.TryParse(Console.ReadLine(), out int age)) return;

            Console.Write("порода: ");
            string breed = Console.ReadLine() ?? string.Empty;

            using (var db = new SqlConnection(connectionString))
            {
                string sql = "insert into dogs (name, age, breed, isadopted) values (@name, @age, @breed, 0)";
                db.Execute(sql, new { name, age, breed });
                Console.WriteLine("успіх.");
            }
            WaitForKey();
        }

        static void ViewDogs(string sql, object? param = null)
        {
            Console.Clear();
            Console.WriteLine("собаки:");

            using (var db = new SqlConnection(connectionString))
            {
                var dogs = db.Query<Dog>(sql, param).AsList();
                if (dogs.Count == 0)
                {
                    Console.WriteLine("пусто.");
                }
                else
                {
                    foreach (var d in dogs)
                    {
                        Console.WriteLine($"{d.Id} | {d.Name} | {d.Age} | {d.Breed} | забрано: {d.IsAdopted}");
                    }
                }
            }
            WaitForKey();
        }

        static void SearchDog()
        {
            Console.Clear();
            Console.WriteLine("пошук");
            Console.WriteLine("1. за клічкою");
            Console.WriteLine("2. за id");
            Console.WriteLine("3. за породою");
            Console.Write("вибір: ");
            
            var choice = Console.ReadLine();

            Console.Write("введіть значення: ");
            var val = Console.ReadLine() ?? string.Empty;

            string sql = "";
            object? param = null;

            switch (choice)
            {
                case "1":
                    sql = "select id, name, age, breed, isadopted from dogs where name like @val";
                    param = new { val = "%" + val + "%" };
                    break;
                case "2":
                    if (!int.TryParse(val, out int idVal)) return;
                    sql = "select id, name, age, breed, isadopted from dogs where id = @id";
                    param = new { id = idVal };
                    break;
                case "3":
                    sql = "select id, name, age, breed, isadopted from dogs where breed like @val";
                    param = new { val = "%" + val + "%" };
                    break;
                default:
                    return;
            }

            ViewDogs(sql, param);
        }

        static void UpdateDog()
        {
            Console.Clear();
            Console.WriteLine("оновлення");

            Console.Write("введіть id: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            using (var db = new SqlConnection(connectionString))
            {
                var dog = db.QueryFirstOrDefault<Dog>("select id, name, age, breed, isadopted from dogs where id = @id", new { id });
                if (dog == null)
                {
                    Console.WriteLine("помилка.");
                    WaitForKey();
                    return;
                }

                Console.Write("нова клічка: ");
                string newName = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(newName)) dog.Name = newName;

                Console.Write("новий вік: ");
                string ageStr = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(ageStr, out int newAge)) dog.Age = newAge;

                Console.Write("нова порода: ");
                string newBreed = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(newBreed)) dog.Breed = newBreed;

                Console.Write("чи забрано? (1 - так, 0 - ні): ");
                string adoptedStr = Console.ReadLine() ?? string.Empty;
                if (adoptedStr == "1") dog.IsAdopted = true;
                if (adoptedStr == "0") dog.IsAdopted = false;

                string sql = "update dogs set name = @name, age = @age, breed = @breed, isadopted = @isadopted where id = @id";
                db.Execute(sql, dog);
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
