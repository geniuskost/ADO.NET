using System;
using Microsoft.Data.SqlClient;

namespace AdoNetExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Підключення до бази даних успішне!");

                    string createTableQuery = @"
                        create table books (
                            id int identity(1,1) primary key,
                            title nvarchar(100) not null,
                            author nvarchar(100) not null,
                            year int not null
                        )";
                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Таблицю 'books' успішно створено.");
                        } 
                        catch (SqlException)
                        {
                        }
                    }

                    using (SqlCommand command = new SqlCommand("delete from books", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string insertQuery = @"
                        insert into books (title, author, year) values 
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

                    string searchQuery = "select id, title, year from books where author like @author";
                    using (SqlCommand command = new SqlCommand(searchQuery, connection))
                    {
                        command.Parameters.AddWithValue("@author", "%" + searchAuthor + "%");

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("\nРезультати пошуку:");
                            bool found = false;
                            while (reader.Read())
                            {
                                found = true;
                                Console.WriteLine($"Id: {reader["id"]}, Назва: {reader["title"]}, Рік: {reader["year"]}");
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
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
