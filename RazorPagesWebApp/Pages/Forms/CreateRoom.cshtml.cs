using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml.Linq;

namespace RazorPagesWebApp.Pages.Forms
{
    public class CreateRoomModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The list is required")]
        public required string InputList { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The pick order is required")]
        public string PickOrder { get; set; } = "123312123123";

        public CreateRoomModel()
        {
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            CreateRoomInputModel createRoomInputModel = UserInputService.PopulateCreateRoomInputModel(InputList, PickOrder);

            TempData["CreateRoomInputModel"] = JsonSerializer.Serialize(createRoomInputModel);

            return RedirectToPage("/GameRoom/Index",
                new {
                    SessionId = Guid.NewGuid().ToString()
                });

        }
    }
}
