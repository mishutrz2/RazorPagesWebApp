using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services;
using RazorPagesWebApp.Services.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public string PickOrder { get; set; } = "123321123123";

        [BindProperty]
        public bool RandomizeCaptainsOrder { get; set; } = true;

        [BindProperty]
        public Guid SessionId { get; set; } // Property to store the session ID

        protected readonly ISessionService _sessionService;

        public CreateRoomModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
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

            Guid newRoomId = Guid.NewGuid();

            SessionId =_sessionService.CreateSession(newRoomId, createRoomInputModel).RoomId;

            //return RedirectToPage("/RoomCreated", new { roomId = SessionId });
            return RedirectToPage("/GameRoom/Index", new { SessionId = SessionId, PlayerName = createRoomInputModel.PickOrder });

        }
    }
}
