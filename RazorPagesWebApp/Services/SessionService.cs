using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;
using System.Collections.Concurrent;

namespace RazorPagesWebApp.Services
{
    public class SessionService : ISessionService
    {
        private readonly ConcurrentDictionary<Guid, Session> _sessions;
        private readonly object _lock = new object();

        public SessionService()
        {
            _sessions = new ConcurrentDictionary<Guid, Session>();

            // Start a background task to periodically check for expired sessions
            Task.Run(() => RemoveExpiredSessionsLoop());
        }

        public Session CreateSession(Guid roomId, CreateRoomInputModel createRoomInputModel)
        {

            var session = new Session { RoomId = roomId, CreateRoomInputModel = createRoomInputModel };

            // first, add captains to the 3 teams
            session.TeamOne.Add(createRoomInputModel.Captains[0]);
            session.TeamTwo.Add(createRoomInputModel.Captains[1]);
            session.TeamThree.Add(createRoomInputModel.Captains[2]);

            _sessions.TryAdd(roomId, session);
            return session;

        }

        public Session GetSession(Guid roomId)
        {

            _sessions.TryGetValue(roomId, out var session);
            return session;

        }

        public bool JoinSession(Guid roomId, string playerName)
        {
            playerName = playerName.ToLower();


            var session = GetSession(roomId);
            if (session != null)
            {
                return session.AddPlayer(playerName);
            }
            return false; // Session not found

        }

        public bool AreThereEnoughCaptains(CreateRoomInputModel createRoomInputModel)
        {
            if (createRoomInputModel.Captains.Count == 3)
            {
                return true;
            }

            return false;
        }

        public bool IsNameOnTheList(Guid roomId, string playerOrCaptainName)
        {
            playerOrCaptainName = playerOrCaptainName.ToLower();


            var session = GetSession(roomId);
            if (session != null)
            {
                if (/*session.CreateRoomInputModel.Players.Contains(playerOrCaptainName) || */session.CreateRoomInputModel.Captains.Contains(playerOrCaptainName))
                {
                    return true;
                }
            }
            return false; // Session not found

        }

        public bool IsNameAlreadyInTheRoom(Guid roomId, string playerOrCaptainName)
        {
            playerOrCaptainName = playerOrCaptainName.ToLower();


            var session = GetSession(roomId);
            if (session != null)
            {
                if (session.Players.Contains(playerOrCaptainName) || session.Captains.Contains(playerOrCaptainName))
                {
                    return true;
                }
            }
            return false; // Session not found

        }

        public int GetPlayerOrderOfChoosing(Guid roomId, string playerOrCaptainName)
        {
            playerOrCaptainName = playerOrCaptainName.ToLower();


            var session = GetSession(roomId);
            if (session != null)
            {
                if (session.CreateRoomInputModel.Captains[0] == playerOrCaptainName)
                {
                    return 1;
                }
                else if (session.CreateRoomInputModel.Captains[1] == playerOrCaptainName)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            return 4; // Session not found

        }

        // methods used to remove expired sessions
        private async Task RemoveExpiredSessionsLoop()
        {
            while (true)
            {
                RemoveExpiredSessions();
                await Task.Delay(TimeSpan.FromMinutes(30)); // Check for expired sessions every 30 minutes
            }
        }

        private void RemoveExpiredSessions()
        {

            var currentTime = DateTime.UtcNow;
            foreach (var kvp in _sessions)
            {
                var session = kvp.Value;
                if (currentTime - session.CreationTime > TimeSpan.FromMinutes(30))
                {
                    _sessions.TryRemove(kvp.Key, out _);
                }
            }

        }
    }
}
