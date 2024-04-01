using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebApp.Pages.GameRoom
{
    public class IndexModel : PageModel
    {
        public void OnGet(string sessionId)
        {
            ViewData["SessionId"] = sessionId;
        }
    }
}
