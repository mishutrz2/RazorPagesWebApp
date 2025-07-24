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

        [BindProperty]
        public bool RandomizeCaptainsOrder { get; set; } = true;

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
                return Page(); // Or return another IActionResult to handle the invalid state
            }

            CreateRoomInputModel createRoomInputModel = UserInputService.PopulateCreateRoomInputModel(InputList, PickOrder);

            if (RandomizeCaptainsOrder)
            {
                var rng = new Random();
                createRoomInputModel.Captains = createRoomInputModel.Captains
                    .OrderBy(x => rng.Next())
                    .ToList();
            }

            TempData["CreateRoomInputModel"] = JsonSerializer.Serialize(createRoomInputModel);

            return RedirectToPage("/GameRoom/Index",
                new {
                    SessionId = Guid.NewGuid().ToString()
                });

        }
    }
}
