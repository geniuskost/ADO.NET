using System;

namespace BoardGamesManagement.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
    }
}
