using RazorPagesWebApp.Models;

namespace RazorPagesWebApp.Services.Interfaces
{
    public interface ISessionService
    {
        Session CreateSession(Guid roomId, CreateRoomInputModel createRoomInputModel);
        Session GetSession(Guid roomId);
    }
}
