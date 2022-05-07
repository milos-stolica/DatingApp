using DatingApp.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DatingApp.API.SignalR.Hubs
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private PresenceTracker presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            this.presenceTracker = presenceTracker;
        }

        public async override Task OnConnectedAsync()
        {
            var username = Context.User.GetUsername();
            var isOnline = await presenceTracker.UserConnected(username, Context.ConnectionId);

            if(isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", username);
            }

            var onlineUsers = await presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("SendOnlineUsers", onlineUsers);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.GetUsername();
            var isOffline = await presenceTracker.UserDisconnected(username, Context.ConnectionId);

            if(isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", username);
            }

            //var onlineUsers = await presenceTracker.GetOnlineUsers();
            //await Clients.All.SendAsync("OnlineUsersChanged", onlineUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }

}
