﻿using Microsoft.AspNetCore.SignalR;
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
        private static string adminAvatarImgUrl = "https://freerangestock.com/sample/119157/business-man-profile-vector.jpg";

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
            var currentSession = _sessionService.GetSession(new Guid(sessionId));

            // Add the connection to the group based on the session ID
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            // Map the connection ID to the session ID
            UserSessionMap.AddOrUpdate(Context.ConnectionId, sessionId, (key, value) => sessionId);

            // Optionally, you can notify clients that the user has joined the session
            await Clients.Group(sessionId).SendAsync("UserJoined", user);

            if (currentSession.CreateRoomInputModel.Captains.Contains(user) && !currentSession.Captains.Contains(user))
            {
                currentSession.Captains.Add(user);
            }

            if (!currentSession.HasStarted && currentSession.Captains.Count == 3 && currentSession.CreateRoomInputModel.Captains.Contains(user))
            {
                currentSession.HasStarted = true;
                await Clients.Group(sessionId).SendAsync("ReceiveMessage", "", $"Au intrat capitanii. {currentSession.CreateRoomInputModel.Captains[0]} alege primul", adminAvatarImgUrl);

                await Clients.Group(sessionId).SendAsync("UnlockTopList");
            }
        }

        public async Task ChoosePlayer(string sessionId, string user, string currentUserIndex, string selectedPlayerName, string nextUserChoosingOrder)
        {
            var session = _sessionService.GetSession(new Guid(sessionId));
            if (session == null)
            {
                await Task.CompletedTask;
                return;
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

            await Clients.Group(sessionId).SendAsync("UpdateTopListAndTeams", currentUserIndex, selectedPlayerName, nextUserChoosingOrder);

            await Clients.Group(sessionId).SendAsync("ReceiveMessage", "", $"{user} l-a ales pe {selectedPlayerName}", adminAvatarImgUrl);

            if (session.TeamOne.Count == 5 && session.TeamTwo.Count == 5 && session.TeamThree.Count == 5)
            {
                var adminAvatarImgUrl = "https://freerangestock.com/sample/119157/business-man-profile-vector.jpg";
                await Clients.Group(sessionId).SendAsync("ReceiveMessage", "", "Gata echipele. Spor la joaca!", adminAvatarImgUrl);
            }
        }

        public async Task LeaveChat(string sessionId, string user)
        {
            // Send the message to all clients in the same group (same sessionId)
            await Clients.OthersInGroup(sessionId).SendAsync("ReceiveMessage", "", $"!!! !!! !!! - INVALID ROOM ---> {user} has disconnected", adminAvatarImgUrl);
            await Clients.OthersInGroup(sessionId).SendAsync("RemoveFromConnectedList", user);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                if (UserSessionMap.TryRemove(Context.ConnectionId, out var sessionId))
                {
                    // Remove the connection from the group when disconnected
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
                    // Now you can use the sessionId as needed
                    Console.WriteLine($"Disconnected SessionId: {sessionId}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it in a way that makes sense for your application
                Console.WriteLine($"An error occurred while handling disconnection: {ex.Message}");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }
        }
    }
}
