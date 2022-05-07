using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Entities
{
    public class MessageHubGroup
    {
        public MessageHubGroup()
        {
        }

        public MessageHubGroup(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; }

        public ICollection<MessageHubConnection> Connections { get; set; } = new List<MessageHubConnection>();
    }
}
