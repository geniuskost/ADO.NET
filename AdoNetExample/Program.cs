using System;
using Microsoft.Data.SqlClient;

namespace AdoNetExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Рядок підключення. Для прикладу використаємо LocalDB (SQL Server)
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Підключення до бази даних успішне!");

                    // 1. Створення таблиці (Предметна область: Книги)
                    string createTableQuery = @"
                        CREATE TABLE Books (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(100) NOT NULL,
                            Author NVARCHAR(100) NOT NULL,
                            Year INT NOT NULL
                        )";
                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Таблицю 'Books' успішно перевірено/створено.");
                    }

                    using (SqlCommand command = new SqlCommand("DELETE FROM Books", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string insertQuery = @"
                        INSERT INTO Books (Title, Author, Year) VALUES 
                        ('1984', 'Джордж Орвелл', 1949),
                        ('Майстер і Маргарита', 'Михайло Булгаков', 1967),
                        ('Кобзар', 'Тарас Шевченко', 1840)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Таблицю заповнено. Додано записів: {rowsAffected}");
                    }

                    Console.WriteLine("\nВведіть ім'я автора або його частину для пошуку (наприклад, 'Орвелл'):");
                    string searchAuthor = Console.ReadLine();

                    string searchQuery = "SELECT Id, Title, Year FROM Books WHERE Author LIKE @Author";
                    using (SqlCommand command = new SqlCommand(searchQuery, connection))
                    {
                        // Використання параметрів для уникнення SQL Injection
                        command.Parameters.AddWithValue("@Author", "%" + searchAuthor + "%");

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("\nРезультати пошуку:");
                            bool found = false;
                            while (reader.Read())
                            {
                                found = true;
                                Console.WriteLine($"Id: {reader["Id"]}, Назва: {reader["Title"]}, Рік: {reader["Year"]}");
                            }
                            
                            if (!found)
                            {
                                Console.WriteLine("За вашим запитом нічого не знайдено.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при роботі з БД: {ex.Message}");
                }
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
