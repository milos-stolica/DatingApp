using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int userId, int likedUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<AppUser> GetUserWithLikedBy(int userId);

        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);

        void AddLike(UserLike like);
    }
}
