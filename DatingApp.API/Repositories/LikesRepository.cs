using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLike(int userId, int likedUserId)
        {
            return await context.Likes.FindAsync(userId, likedUserId);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.Users
                     .Include(user => user.LikedUsers)
                     .FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<AppUser> GetUserWithLikedBy(int userId)
        {
            return await context.Users
                     .Include(user => user.LikedByUsers)
                     .FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = context.Users.AsQueryable();
            var likes = context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                users = likes.Where(like => like.SourceUserId == likesParams.UserId)
                             .Select(like => like.LikedUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                users = likes.Where(like => like.LikedUserId == likesParams.UserId)
                             .Select(like => like.SourceUser);
            }

            var likeDTOs = users.OrderBy(user => user.UserName)
                                .Select(user => new LikeDTO()
                                {
                                    Id = user.Id,
                                    Username = user.UserName,
                                    Age = user.DateOfBirth.CalculateAge(),
                                    KnownAs = user.KnownAs,
                                    PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                                    City = user.City
                                });

            return await PagedList<LikeDTO>.CreatePagedList(likeDTOs, likesParams.PageNumber, likesParams.PageSize);
        }

        public void AddLike(UserLike like)
        {
            context.Likes.Add(like);
        }

        //private LikeDTO MapToLikeDTO(AppUser user)
        //{
        //    return new LikeDTO()
        //    {
        //        Id = user.Id,
        //        Username = user.UserName,
        //        Age = user.DateOfBirth.CalculateAge(),
        //        KnownAs = user.KnownAs,
        //        PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
        //        City = user.City
        //    };
        //}
    }
}
