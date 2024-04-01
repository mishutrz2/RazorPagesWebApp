using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebApp.Pages
{
    public class RoomCreatedModel : PageModel
    {
        public string RoomId { get; private set; }

        public void OnGet(Guid roomId)
        {
            RoomId = roomId.ToString();
        }
    }
}
