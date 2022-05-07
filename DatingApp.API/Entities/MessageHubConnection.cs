using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Entities
{
    public class MessageHubConnection
    {
        public MessageHubConnection()
        {
        }

        public MessageHubConnection(string username, string messageHubConnectionId)
        {
            Username = username;
            MessageHubConnectionId = messageHubConnectionId;
        }

        public string MessageHubConnectionId { get; set; }

        [Required]
        public string Username { get; set; }

        public MessageHubGroup Group { get; set; }

        [Required]
        public string GroupId { get; set; }
    }
}
