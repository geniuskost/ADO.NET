using System;
using System.Collections.Generic;

namespace BoardGamesManagement.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
