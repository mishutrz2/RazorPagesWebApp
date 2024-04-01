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

            // Validate input (e.g., check if session ID and name are not empty)
            Session session = _sessionService.GetSession(sessionGuid);

            if (session == null)
            {
                return RedirectToPage("/RoomNotFound");
            }

            // Redirect to the game room page with the session ID
            return RedirectToPage("/GameRoom/Index", new { sessionId });
        }
    }
}
