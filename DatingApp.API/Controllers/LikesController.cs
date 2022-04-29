using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRepo;
        private readonly ILikesRepository likesRepo;

        public LikesController(IUserRepository userRepo, ILikesRepository likesRepo)
        {
            this.userRepo = userRepo;
            this.likesRepo = likesRepo;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var userId = User.GetUserId();
            var userUsername = User.GetUsername();

            var likedUser = await userRepo.GetUserByUsernameAsync(username);

            if(likedUser == null)
            {
                return NotFound();
            }

            if(userUsername == username)
            {
                return BadRequest("You cannot like yourself.");
            }

            if(await likesRepo.GetUserLike(userId, likedUser.Id) != null)
            {
                return BadRequest($"You already likes {likedUser.KnownAs}");
            }

            var userLike = new UserLike()
            {
                SourceUserId = userId,
                LikedUserId = likedUser.Id
            };

            AppUser user = await likesRepo.GetUserWithLikes(userId);

            user.LikedUsers.Add(userLike);

            if(await userRepo.SaveAllChangesAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes(string predicate)
        {
            return Ok(await likesRepo.GetUserLikes(predicate, User.GetUserId()));
        }

    }
}
