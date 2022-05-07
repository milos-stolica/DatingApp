using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface IMessageHubRepository
    {
        void AddGroup(MessageHubGroup group);

        Task<MessageHubGroup> GetGroup(string name);

        Task<bool> SaveAllChangesAsync();

        IEnumerable<string> GetGroupConnections(MessageHubGroup group);

        Task RemoveConnectionFromGroup(MessageHubGroup group, string connectionId);

        void StopTrackingAll();
    }
}
