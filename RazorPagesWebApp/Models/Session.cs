using Microsoft.AspNetCore.SignalR;
using RazorPagesWebApp.Hubs;

namespace RazorPagesWebApp.Models
{
    public class Session
    {
        public Guid RoomId { get; set; }

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public CreateRoomInputModel CreateRoomInputModel { get; set; }

        public List<string> Players { get; set; } = new List<string>(); // List of players

        public List<string> Captains { get; set; } = new List<string>(); // List of captains

        public List<string> TeamOne { get; set; } = new List<string>(); // Team 1

        public List<string> TeamTwo { get; set; } = new List<string>(); // Team 2

        public List<string> TeamThree { get; set; } = new List<string>(); // Team 3

        public bool AddPlayer(string playerName)
        {
            if (!Players.Contains(playerName))
            {
                Players.Add(playerName);
                return true;
            }
            return false; // Player already exists
        }

        public bool AddCaptain(string captainName)
        {
            if (!Captains.Contains(captainName))
            {
                Captains.Add(captainName);
                return true;
            }
            return false; // Captain already exists
        }
    }
}
