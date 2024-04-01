using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApp.Services.Interfaces;

namespace RazorPagesWebApp.Pages.GameRoom
{
    public class IndexModel : PageModel
    {
        //private readonly ISessionService _sessionService;

        [BindProperty(SupportsGet = true)]
        public string SessionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PlayerName { get; set; }

        /*public IndexModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }*/

        public void OnGet()
        {

        }
    }
}
