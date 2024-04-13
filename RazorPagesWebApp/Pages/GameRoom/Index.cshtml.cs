using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;

namespace RazorPagesWebApp.Pages.GameRoom
{
    public class IndexModel : PageModel
    {
        private readonly ISessionService _sessionService;

        public Session CurrentSession { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SessionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PlayerName { get; set; }

        public int PlayerOrder { get; set; }


        public IndexModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult OnGet()
        {
            CurrentSession = _sessionService.GetSession(new Guid(SessionId));

            PlayerOrder = _sessionService.GetPlayerOrderOfChoosing(new Guid(SessionId), PlayerName);

            if (CurrentSession == null) {
                return RedirectToPage("../RoomNotFound");
            }

            return Page();
        }
    }
}
