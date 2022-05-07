using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.SignalR
{
    //this is bad options, especially for web farms. Use database storage or even beter use Redis
    public class PresenceTracker
    {
        private Dictionary<string, HashSet<string>> onlineUsers;
        private object syncObject;

        public PresenceTracker()
        {
            onlineUsers = new Dictionary<string, HashSet<string>>();
            syncObject = new object();
        }

        public Task<bool> UserConnected(string username, string connectionId) 
        {
            lock (syncObject)
            {
                if (onlineUsers.ContainsKey(username))
                {
                    onlineUsers[username].Add(connectionId);
                    return Task.FromResult(false);
                }
                else
                {
                    onlineUsers.Add(username, new HashSet<string>() { connectionId });
                    return Task.FromResult(true);
                }
            }
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            lock (syncObject)
            {
                if (!onlineUsers.ContainsKey(username))
                {
                    return Task.FromResult(false);
                }

                onlineUsers[username].Remove(connectionId);
                if(onlineUsers[username].Count == 0)
                {
                    onlineUsers.Remove(username);
                    return Task.FromResult(true);
                }     
            }

            return Task.FromResult(false);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] connectedUsers;
            lock (syncObject)
            {
                connectedUsers = onlineUsers.Keys.OrderBy(username => username).ToArray();
            }

            return Task.FromResult(connectedUsers);
        }

        public Task<IEnumerable<string>> GetConnectionsForUser(string username)
        {
            IEnumerable<string> connections;
            lock(syncObject)
            {
                connections = onlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connections);
        }
    }
}
