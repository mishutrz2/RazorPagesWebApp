namespace RazorPagesWebApp.Models
{
    public class Session
    {
        public Guid RoomId { get; set; }

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public CreateRoomInputModel CreateRoomInputModel { get; set; }

        public List<string> Players { get; set; } = new List<string>(); // List of players

        public List<string> Captains { get; set; } = new List<string>(); // List of captains

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
