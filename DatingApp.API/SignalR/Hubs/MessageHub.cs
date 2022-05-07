using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.SignalR.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly IMessageHubRepository messageHubRepository;
        private readonly IMapper mapper;
        private readonly ILogger<MessageHub> logger;
        private readonly PresenceTracker presenceTracker;
        private readonly IHubContext<PresenceHub> presenceHubContext;

        public MessageHub(IMessageRepository messageRepository, 
                          IUserRepository userRepository,
                          IMessageHubRepository messageHubRepository,
                          IMapper mapper,
                          ILogger<MessageHub> logger,
                          PresenceTracker presenceTracker,
                          IHubContext<PresenceHub> presenceHubContext)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.messageHubRepository = messageHubRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.presenceTracker = presenceTracker;
            this.presenceHubContext = presenceHubContext;
        }

        public async override Task OnConnectedAsync()
        {
            var currentUsername = Context.User.GetUsername();
            //when connecting on hub, send person to chat with in query string (when making connection)
            var otherUsername = Context.GetHttpContext().Request.Query["user"].ToString();
            var groupName = GetGroupName(currentUsername, otherUsername);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddGroupAsync(groupName);
            //notify group members about other users get in group
            await Clients.Clients(group.Connections.Where(conn => conn.Username != currentUsername)
                                                   .Select(conn => conn.MessageHubConnectionId))
                         .SendAsync("NewUserInGroup", currentUsername);


            var messages = await messageRepository.GetMessageThread(currentUsername, otherUsername);
            await Clients.Caller.SendAsync("GetMessageThreadOnConnected", messages);

            //await Clients.Group(groupName).SendAsync("GetMessageThreadOnConnected", messages);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var currentUsername = Context.User.GetUsername();
            //when connecting on hub, send person to chat with in query string (when making connection)
            var otherUsername = Context.GetHttpContext().Request.Query["user"].ToString();
            var groupName = GetGroupName(currentUsername, otherUsername);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName); //this is automatically done, can delete line
            await RemoveConnectionFromGroupAsync(groupName);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var currentUsername = Context.User.GetUsername();

            if (currentUsername == createMessageDTO.RecipientUsername.Trim().ToLower())
            {
                throw new HubException("You cannot send message to yourself.");
            }

            //this is not option because senderPhotoUrl is not present during mapping
            //var currentUserId = User.GetUserId();
            //var currentUser = await userRepository.GetUserByIdAsync(currentUserId);
            var currentUser = await userRepository.GetUserByUsernameAsync(currentUsername);
            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null)
            {
                throw new HubException("Not found user.");
            }

            var message = new Message()
            {
                SenderUsername = currentUser.UserName,
                //Sender = currentUser,
                RecipientUsername = recipient.UserName,
                //Recipient = recipient,
                Content = createMessageDTO.Content,
                SenderId = currentUser.Id,
                RecipientId = recipient.Id
            };

            var groupName = GetGroupName(currentUsername, createMessageDTO.RecipientUsername);

            if(!await TryMarkMessageAsRead(createMessageDTO.RecipientUsername, message, groupName))
            {
                //todo send notification to user if online
                var recipientConnections = await presenceTracker.GetConnectionsForUser(createMessageDTO.RecipientUsername);

                if(recipientConnections != null)
                {
                    await presenceHubContext.Clients.Clients(recipientConnections)
                        .SendAsync("MessageNotification", new { username = currentUser.UserName, 
                                                                knownAs = currentUser.KnownAs });
                }

            }

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllChangesAsync())
            {
                var messageDTO = mapper.Map<MessageDTO>(message);

                await Clients.Group(groupName).SendAsync("NewMessage", messageDTO);
                return;
            }

            throw new HubException("Failed sending message.");
        }

        private async Task<bool> TryMarkMessageAsRead(string username, Message message, string groupName)
        {
            var group = await messageHubRepository.GetGroup(groupName);

            if (group != null)
            {
                IEnumerable<string> groupUsernames = messageHubRepository.GetGroupConnections(group);

                if (groupUsernames.Contains(username))
                {
                    message.DateRead = DateTime.UtcNow;
                    return true;
                }
            }

            return false;
        }

        private string GetGroupName(string currentUsername, string otherUsername)
        {
            var stringCompare = string.CompareOrdinal(currentUsername, otherUsername) < 0;

            return stringCompare ? $"{currentUsername}-{otherUsername}" :
                                   $"{otherUsername}-{currentUsername}";
        }

        private async Task<MessageHubGroup> AddGroupAsync(string groupName)
        {
            bool saved = false;
            int maxTryNumber = 5;
            while(!saved && --maxTryNumber > 0)
            {
                try
                {
                    var group = await TryAddGroup(groupName);
                    saved = await messageHubRepository.SaveAllChangesAsync();
                    return group;
                }
                catch (DbUpdateException ex)
                {
                    messageHubRepository.StopTrackingAll();
                    saved = false;
                    logger.LogError(ex, ex.Message);
                }
            }

            throw new HubException("Failed adding message hub group.");
        }

        private async Task<MessageHubGroup> TryAddGroup(string groupName)
        {
            var group = await messageHubRepository.GetGroup(groupName);

            var connection = new MessageHubConnection(Context.User.GetUsername(), Context.ConnectionId);

            if (group == null)
            {
                group = new MessageHubGroup(groupName);
                messageHubRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return group;
        }

        private async Task RemoveConnectionFromGroupAsync(string groupName)
        {
            //this should have been done directly on Connections table, there wasnt necessity for doing this way
            var group = await messageHubRepository.GetGroup(groupName);

            if (group != null)
            {
                await messageHubRepository.RemoveConnectionFromGroup(group, Context.ConnectionId);
            }

            await messageHubRepository.SaveAllChangesAsync();
        }
    }

}
