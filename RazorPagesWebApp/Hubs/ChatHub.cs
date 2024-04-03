using Microsoft.AspNetCore.SignalR;
using RazorPagesWebApp.Models;
using RazorPagesWebApp.Services.Interfaces;
using System.Collections.Concurrent;

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

            // Broadcast the message to all clients in the chat room
            //await Clients.Others.SendAsync("ReceiveMessage", user, message, avatarUrl);
        }

        public async Task JoinSession(string sessionId)
        {
            // Add the connection to the group based on the session ID
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            // Map the connection ID to the session ID
            UserSessionMap.AddOrUpdate(Context.ConnectionId, sessionId, (key, value) => sessionId);

            // Optionally, you can notify clients that the user has joined the session
            await Clients.Group(sessionId).SendAsync("UserJoined", Context.ConnectionId);
        }

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
