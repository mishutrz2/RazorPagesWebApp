using Microsoft.AspNetCore.SignalR;

namespace RazorPagesWebApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string avatarUrl)
        {
            // Broadcast the message to all clients in the chat room
            await Clients.Others.SendAsync("ReceiveMessage", user, message, avatarUrl);
        }

        public async Task JoinChat(string user)
        {
            // Notify other clients that a new user has joined the chat room
            await Clients.Others.SendAsync("UserJoined", user);
        }
    }
}
