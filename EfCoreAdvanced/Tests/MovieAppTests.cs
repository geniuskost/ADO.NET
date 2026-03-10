using System;
using System.Linq;
using Xunit;
using EfCoreAdvanced.Data;
using EfCoreAdvanced.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreAdvanced.Tests
{
    public class MovieAppTests : IDisposable
    {
        private readonly AdvancedDbContext _db;

        public MovieAppTests()
        {
            _db = new AdvancedDbContext();
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            _db.Database.ExecuteSqlRaw(@"
                create view users_movies_view as
                select u.username, m.title as movietitle
                from users u
                join movies m on u.id = m.userid;
            ");

            _db.Database.ExecuteSqlRaw(@"
                create procedure add_user
                    @username nvarchar(max),
                    @email nvarchar(max),
                    @password nvarchar(max)
                as
                begin
                    insert into users (username, email, password)
                    values (@username, @email, @password);
                end;
            ");
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }

        [Fact]
        public void CreateUser_Test()
        {
            var user = new User { Username = "testuser", Email = "test@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var added = _db.Users.FirstOrDefault(u => u.Username == "testuser");
            Assert.NotNull(added);
            Assert.Equal("test@mail.com", added.Email);
        }

        [Fact]
        public void ReadUser_Test()
        {
            var user = new User { Username = "readuser", Email = "read@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var found = _db.Users.Find(user.Id);
            Assert.NotNull(found);
            Assert.Equal("readuser", found.Username);
        }

        [Fact]
        public void UpdateUser_Test()
        {
            var user = new User { Username = "upduser", Email = "upd@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            user.Password = "newpass";
            _db.SaveChanges();

            var found = _db.Users.Find(user.Id);
            Assert.Equal("newpass", found?.Password);
        }

        [Fact]
        public void DeleteUser_Test()
        {
            var user = new User { Username = "deluser", Email = "del@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            _db.Users.Remove(user);
            _db.SaveChanges();

            var found = _db.Users.Find(user.Id);
            Assert.Null(found);
        }

        [Fact]
        public void CreateMovie_Test()
        {
            var user = new User { Username = "movuser", Email = "mov@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var movie = new Movie { Title = "test movie", ReleaseYear = 2024, UserId = user.Id };
            _db.Movies.Add(movie);
            _db.SaveChanges();

            var added = _db.Movies.FirstOrDefault(m => m.Title == "test movie");
            Assert.NotNull(added);
            Assert.Equal(2024, added.ReleaseYear);
        }

        [Fact]
        public void View_UsersMovies_Test()
        {
            var user = new User { Username = "viewuser", Email = "view@mail.com", Password = "123" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var movie = new Movie { Title = "view movie", ReleaseYear = 2024, UserId = user.Id };
            _db.Movies.Add(movie);
            _db.SaveChanges();

            var viewResult = _db.UserMovieViews.ToList();
            Assert.NotEmpty(viewResult);
            Assert.Contains(viewResult, v => v.Username == "viewuser" && v.MovieTitle == "view movie");
        }

        [Fact]
        public void Procedure_AddUser_Test()
        {
            _db.Database.ExecuteSqlRaw("exec add_user @p0, @p1, @p2", "procuser", "proc@mail.com", "123");

            var added = _db.Users.FirstOrDefault(u => u.Username == "procuser");
            Assert.NotNull(added);
            Assert.Equal("proc@mail.com", added.Email);
        }
    }
}
