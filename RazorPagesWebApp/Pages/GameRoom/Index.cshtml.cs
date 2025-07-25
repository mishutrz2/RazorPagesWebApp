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

        public CreateRoomInputModel CreateRoomInputModel { get; set; }

        public IndexModel()
        {
            
        }

        public IActionResult OnGet()
        {

            if (!(TempData["CreateRoomInputModel"] is string modelJson))
            {
                return RedirectToPage("/Forms/CreateRoom"); // Redirect to the form if TempData updated
            }
            else
            {
                CreateRoomInputModel = JsonSerializer.Deserialize<CreateRoomInputModel>(modelJson);
            }

            if (CreateRoomInputModel.Captains.Count != 0 && CreateRoomInputModel.Captains.Count != 3)
            {
                return RedirectToPage("/Forms/CreateRoom"); // Redirect to the form if error with captains setup
            }

            return Page();
        }
    }
}
