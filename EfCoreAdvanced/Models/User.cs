using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfCoreAdvanced.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MinLength(1)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
