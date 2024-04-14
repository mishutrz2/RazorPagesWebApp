using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace TeamPickChatWebApp.Pages
{
    public class JoinRoomModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Enter room id")]
        public string EnterSessionId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Enter your name")]
        public string InputName { get; set; }

        protected readonly ISessionService _sessionService;

        public JoinRoomModel(ISessionService sessionService)
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

            Guid sessionGuid;
            try
            {
                sessionGuid = new Guid(EnterSessionId);
            }
            catch (FormatException)
            {
                sessionGuid = Guid.Empty;
            }

            // Validate input (e.g., check if session ID is not empty)
            Session session = _sessionService.GetSession(sessionGuid);

            if (session == null)
            {
                return RedirectToPage("/RoomNotFound");
            }

            // If user is not on the list, reject their request
            if (!_sessionService.IsNameOnTheList(sessionGuid, InputName.ToLower()))
            {
                return RedirectToPage("/UserNotAllowedInThisRoom");
            }

            // If user is already connected, reject their request
            if (_sessionService.IsNameAlreadyInTheRoom(sessionGuid, InputName.ToLower()))
            {
                return RedirectToPage("/UserAlreadyInThisRoom");
            }

            // If everything is ok, connect the user to the room
            _sessionService.JoinSession(sessionGuid, InputName.ToLower());

            // Redirect to the game room page with the session ID
            return RedirectToPage("/GameRoom/Index", new { SessionId = EnterSessionId, PlayerName = InputName.ToLower() });
        }
    }
}
