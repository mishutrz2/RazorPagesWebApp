using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using System.Text.Json;

namespace RazorPagesWebApp.Pages.GameRoom
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SessionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PickOrder { get; set; }

        public CreateRoomInputModel CreateRoomInputModel { get; set; }

        public IndexModel()
        {
            
        }

        public IActionResult OnGet()
        {
            if (TempData["CreateRoomInputModel"] is string modelJson)
            {
                CreateRoomInputModel = JsonSerializer.Deserialize<CreateRoomInputModel>(modelJson);
            }

            return Page();
        }
    }
}
