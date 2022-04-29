using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int userId, int likedUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId);
    }
}
