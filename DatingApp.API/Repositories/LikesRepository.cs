using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId)
        {
            if(predicate == null)
            {
                return new List<LikeDTO>();
            }

            if(predicate != "liked" && predicate != "likedBy")
            {
                return new List<LikeDTO>();
            }

            var users = context.Users.AsQueryable();
            var likes = context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                users = likes.Where(like => like.SourceUserId == userId)
                             .Select(like => like.LikedUser);
            }

            if (predicate == "likedBy")
            {
                users = likes.Where(like => like.LikedUserId == userId)
                             .Select(like => like.SourceUser);
            }

            return await users.OrderBy(user => user.UserName)
                              .Select(user => new LikeDTO()
                              {
                                  Id = user.Id,
                                  Username = user.UserName,
                                  Age = user.DateOfBirth.CalculateAge(),
                                  KnownAs = user.KnownAs,
                                  PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                                  City = user.City
                              })
                              .ToListAsync();
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
