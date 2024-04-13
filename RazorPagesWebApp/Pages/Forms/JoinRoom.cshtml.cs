using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;

namespace TeamPickChatWebApp.Pages
{
    public class JoinRoomModel : PageModel
    {
        protected readonly ISessionService _sessionService;

        public JoinRoomModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string sessionId, string name)
        {
            Guid sessionGuid;
            try
            {
                sessionGuid = new Guid(sessionId);
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

            name = name.ToLower();

            // If user is not on the list, reject their request
            if (!_sessionService.IsNameOnTheList(sessionGuid, name))
            {
                return RedirectToPage("/UserNotAllowedInThisRoom");
            }

            // If user is already connected, reject their request
            if (_sessionService.IsNameAlreadyInTheRoom(sessionGuid, name))
            {
                return RedirectToPage("/UserAlreadyInThisRoom");
            }

            // If everything is ok, connect the user to the room
            _sessionService.JoinSession(sessionGuid, name);

            // Redirect to the game room page with the session ID
            return RedirectToPage("/GameRoom/Index", new { SessionId = sessionId, PlayerName = name });
        }
    }
}
