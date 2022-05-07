using DatingApp.API.Data;
using DatingApp.API.Entities;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories
{
    public class MessageHubRepository : IMessageHubRepository
    {
        private readonly DataContext context;

        public MessageHubRepository(DataContext context)
        {
            this.context = context;
        }

        public void AddGroup(MessageHubGroup group)
        {
            context.MessageHubGroups.Add(group);
        }

        public async Task<MessageHubGroup> GetGroup(string name)
        {
            return await context.MessageHubGroups
                                    .Include(group => group.Connections)
                                    .SingleOrDefaultAsync(group => group.Name == name);  
        }

        public IEnumerable<string> GetGroupConnections(MessageHubGroup group)
        {
            return group.Connections.Select(conn => conn.Username);
        }

        public async Task RemoveConnectionFromGroup(MessageHubGroup group, string connectionId)
        {
            var connection = await context.MessageHubConnections.FindAsync(connectionId);

            if(connection == null)
            {
                return;
            }

            group.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void StopTrackingAll()
        {
            context.ChangeTracker.Clear();
        }
    }
}
