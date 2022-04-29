namespace DatingApp.API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser;

        public int SourceUserId;

        public AppUser LikedUser;

        public int LikedUserId;
    }
}
