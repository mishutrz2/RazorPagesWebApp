using Microsoft.AspNetCore.Http;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;
using System.Collections.Concurrent;

namespace RazorPagesWebApp.Services
{
    public class SessionService : ISessionService
    {
        private readonly Dictionary<Guid, Session> _sessions;

        public SessionService()
        {
            _sessions = new Dictionary<Guid, Session>();
        }

        public Session CreateSession(Guid roomId, CreateRoomInputModel createRoomInputModel)
        {
            var session = new Session { RoomId = roomId, CreateRoomInputModel = createRoomInputModel };

            // first, add captains to the 3 teams
            session.TeamOne.Add(createRoomInputModel.Captains[0]);
            session.TeamTwo.Add(createRoomInputModel.Captains[1]);
            session.TeamThree.Add(createRoomInputModel.Captains[2]);

            _sessions.Add(roomId, session);
            return session;
        }

        public Session GetSession(Guid roomId)
        {
            _sessions.TryGetValue(roomId, out var session);
            return session;
        }
    }
}
