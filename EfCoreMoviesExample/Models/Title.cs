namespace EfCoreMoviesExample.Models
{
    public class Title
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; } // Тривалість у хвилинах
    }
}
