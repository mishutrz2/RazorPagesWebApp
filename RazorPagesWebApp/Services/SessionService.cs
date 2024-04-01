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
            lock (_lock)
            {
                var session = new Session { RoomId = roomId, CreateRoomInputModel = createRoomInputModel };
                _sessions.TryAdd(roomId, session);
                return session;
            }
        }

        public Session GetSession(Guid roomId)
        {
            lock (_lock)
            {
                _sessions.TryGetValue(roomId, out var session);
                return session;
            }
        }

        public bool JoinSession(Guid roomId, string playerName)
        {
            playerName = playerName.ToLower();

            lock (_lock)
            {
                var session = GetSession(roomId);
                if (session != null)
                {
                    return session.AddPlayer(playerName);
                }
                return false; // Session not found
            }
        }

        public bool ListAlreadyEntered(CreateRoomInputModel createRoomInputModel)
        {
            foreach (var session in _sessions.Values)
            {
                if (createRoomInputModel.EqualsModel(session.CreateRoomInputModel))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsNameOnTheList(Guid roomId, string playerOrCaptainName)
        {
            playerOrCaptainName = playerOrCaptainName.ToLower();

            lock (_lock)
            {
                var session = GetSession(roomId);
                if (session != null)
                {
                    if (session.CreateRoomInputModel.Players.Contains(playerOrCaptainName) || session.CreateRoomInputModel.Captains.Contains(playerOrCaptainName))
                    {
                        return true;
                    }
                }
                return false; // Session not found
            }
        }

        public bool IsNameAlreadyInTheRoom(Guid roomId, string playerOrCaptainName)
        {
            playerOrCaptainName = playerOrCaptainName.ToLower();

            lock (_lock)
            {
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
        }

        // methods used to remove expired sessions
        private async Task RemoveExpiredSessionsLoop()
        {
            while (true)
            {
                RemoveExpiredSessions();
                await Task.Delay(TimeSpan.FromHours(2)); // Check for expired sessions every 2 hours
            }
        }

        private void RemoveExpiredSessions()
        {
            lock (_lock)
            {
                var currentTime = DateTime.UtcNow;
                foreach (var kvp in _sessions)
                {
                    var session = kvp.Value;
                    if (currentTime - session.CreationTime > TimeSpan.FromHours(2))
                    {
                        _sessions.TryRemove(kvp.Key, out _);
                    }
                }
            }
        }
    }
}
