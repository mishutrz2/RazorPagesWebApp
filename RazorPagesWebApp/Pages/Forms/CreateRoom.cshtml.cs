using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services;
using RazorPagesWebApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesWebApp.Pages.Forms
{
    public class CreateRoomModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The list is required")]
        public required string InputList { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The number of repicks is required")]
        [Range(1, 10, ErrorMessage = "Value must be between {1} and {2}")]
        public int NumberOfRepicks { get; set; }

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

            CreateRoomInputModel createRoomInputModel = UserInputService.PopulateCreateRoomInputModel(InputList, NumberOfRepicks);

            Guid newRoomId = Guid.NewGuid();

            if (_sessionService.ListAlreadyEntered(createRoomInputModel))
            {
                return RedirectToPage("/ListAlreadyEntered");
            }

            SessionId =_sessionService.CreateSession(newRoomId, createRoomInputModel).RoomId;

            return RedirectToPage("/RoomCreated", new { roomId = SessionId });
        }
    }
}
