using System;
using System.ComponentModel.DataAnnotations;

namespace EfCoreFluentApi.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ReleaseYear { get; set; }

        public string? Description { get; set; }

        public DateTime AddedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
