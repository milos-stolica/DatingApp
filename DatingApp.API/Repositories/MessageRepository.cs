using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                                .AsNoTracking()
                                .OrderByDescending(message => message.MessageSent)
                                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(message => message.RecipientUsername == messageParams.Username &&
                                                  !message.RecipientDeleted),
                "Outbox" => query.Where(message => message.SenderUsername == messageParams.Username &&
                                                   !message.SenderDeleted),
                _ => query.Where(message => message.RecipientUsername == messageParams.Username &&
                                            !message.RecipientDeleted == false &&
                                            message.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreatePagedList(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            IEnumerable<Message> messages = await context.Messages
                    .Include(message => message.Sender.Photos.Where(photo => photo.IsMain))//this takes everithing from user unneccessarely
                    .Include(message => message.Recipient.Photos.Where(photo => photo.IsMain))
                    .Where(message => message.RecipientUsername == currentUsername &&
                                      !message.RecipientDeleted &&
                                      message.SenderUsername == recipientUsername ||
                                      message.SenderUsername == currentUsername &&
                                      !message.SenderDeleted &&
                                      message.RecipientUsername == recipientUsername)
                    .OrderBy(message => message.MessageSent)
                    .ToListAsync();

            messages.Where(message => message.DateRead == null &&
                                      message.RecipientUsername == currentUsername)
                    .ToList()
                    .ForEach(message => message.DateRead = DateTime.Now);

            if(context.ChangeTracker.HasChanges())
            {
                await SaveAllChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
