namespace RazorPagesWebApp.Models
{
    public class CreateRoomInputModel
    {
        public List<string> Players {  get; set; } = new List<string>();

        public List<string> Captains { get; set; } = new List<string>() { "", "", ""};

        public string PickOrder { get; set; }
    }
}
