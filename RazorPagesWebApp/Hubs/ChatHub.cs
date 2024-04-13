using Microsoft.AspNetCore.SignalR;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace RazorPagesWebApp.Hubs
{
    public partial class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserSessionMap = new ConcurrentDictionary<string, string>();

        protected readonly ISessionService _sessionService;

        public ChatHub(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task SendMessage(string sessionId, string user, string message, string avatarUrl)
        {
            if (!UserSessionMap.TryGetValue(Context.ConnectionId, out var session) || session != sessionId)
            {
                // User not in the session, don't send the message
                return;
            }

            // Send the message to all clients in the same group (same sessionId)
            await Clients.OthersInGroup(sessionId).SendAsync("ReceiveMessage", user, message, avatarUrl);
        }

        public async Task JoinSession(string sessionId, string user)
        {
            // Add the connection to the group based on the session ID
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            // Map the connection ID to the session ID
            UserSessionMap.AddOrUpdate(Context.ConnectionId, sessionId, (key, value) => sessionId);

            // Optionally, you can notify clients that the user has joined the session
            await Clients.Group(sessionId).SendAsync("UserJoined", Context.ConnectionId);

            var currentSession = _sessionService.GetSession(new Guid(sessionId));

            if (currentSession.CreateRoomInputModel.Captains.Contains(user) && !currentSession.Captains.Contains(user))
            {
                currentSession.Captains.Add(user);
            }

            if (currentSession.Captains.Count == 3 && currentSession.CreateRoomInputModel.Captains.Contains(user))
            {
                var adminAvatarImgUrl = "https://freerangestock.com/sample/119157/business-man-profile-vector.jpg";
                await Clients.Group(sessionId).SendAsync("ReceiveMessage", "", "AU INTRAT TOTI CAPITANII! ALEGETI-VA JUCATORII!", adminAvatarImgUrl);

                await Clients.Group(sessionId).SendAsync("UnlockTopList");
            }
        }

        public async Task ChoosePlayer(string sessionId, string user, string currentUserIndex, string selectedPlayerName)
        {
            var session = _sessionService.GetSession(new Guid(sessionId));
            if (session == null)
            {
                await Task.CompletedTask;
            }

            if (session.CreateRoomInputModel.Captains[0] == user && !session.TeamOne.Contains(selectedPlayerName))
            {
                session.TeamOne.Add(selectedPlayerName);
            }
            else if (session.CreateRoomInputModel.Captains[1] == user && !session.TeamTwo.Contains(selectedPlayerName))
            {
                session.TeamTwo.Add(selectedPlayerName);
            }
            else if (session.CreateRoomInputModel.Captains[2] == user && !session.TeamThree.Contains(selectedPlayerName))
            {
                session.TeamThree.Add(selectedPlayerName);
            }

            await Clients.Group(sessionId).SendAsync("UpdateTopListAndTeams", currentUserIndex, selectedPlayerName);
        }


        // /////////// /////////// /////////// /////////// ///////////

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (UserSessionMap.TryRemove(Context.ConnectionId, out var session))
            {
                // Remove the connection from the group when disconnected
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, session);

                // Optionally, you can notify clients that the user has left the session
                await Clients.Group(session).SendAsync("UserLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
