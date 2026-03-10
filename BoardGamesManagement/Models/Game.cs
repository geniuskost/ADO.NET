using System.Collections.Generic;

namespace BoardGamesManagement.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
