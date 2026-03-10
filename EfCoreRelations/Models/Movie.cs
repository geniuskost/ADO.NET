namespace EfCoreRelations.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string? Description { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
