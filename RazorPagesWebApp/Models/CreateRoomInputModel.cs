namespace RazorPagesWebApp.Models
{
    public class CreateRoomInputModel
    {
        public List<string> Players {  get; set; } = new List<string>();

        public List<string> Captains { get; set; } = new List<string>();

        public int NumberOfRepicks { get; set; }

        public bool EqualsModel(CreateRoomInputModel model)
        {
            if (model.Captains.SequenceEqual(this.Captains) && model.Players.SequenceEqual(this.Players))
            {
                return true;
            }

            return false;
        }
    }
}
