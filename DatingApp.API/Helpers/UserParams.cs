using DatingApp.API.Helpers.Pagination;

namespace DatingApp.API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string CurrentUsername { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 99;

        public string OrderBy { get; set; } = "lastActive";
    }

    public class MessageParams :  PaginationParams
    {
        public string Username { get; set; }

        public string Container { get; set; } = "Unread";
    }
}
