using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Repositories.Interfaces;
using DatingApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(ILogger<UsersController> logger,
                               IUserRepository userRepository,
                               IMapper mapper,
                               IPhotoService photoService)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await userRepository.GetMembersAsync());
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await userRepository.GetMembersByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            mapper.Map(memberUpdateDTO, user);

            userRepository.Update(user);
            if(await userRepository.SaveAllChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var result = await photoService.AddPhotoAsync(file);

            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = new Photo()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(!user.Photos.Any())
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await userRepository.SaveAllChangesAsync())
            {
                var photoDTO = mapper.Map<PhotoDTO>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, photoDTO);
            }

            return BadRequest("Problem adding photo.");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if(photo == null)
            {
                return NotFound();
            }

            if(photo.IsMain)
            {
                return BadRequest("Photo is already your main photo.");
            }

            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if(currentMain != null)
            {
                currentMain.IsMain = false;
            }

            photo.IsMain = true;

            if(await userRepository.SaveAllChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo.");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if(photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("Cannot delte main photo.");
            }

            if(photo.PublicId != null)
            {
                DeletionResult deletionResult =  await photoService.DeletePhotoAsync(photo.PublicId);
                if(deletionResult.Error != null)
                {
                    return BadRequest(deletionResult.Error.Message);
                }
            }

            user.Photos.Remove(photo);

            if(await userRepository.SaveAllChangesAsync())
            {
                return Ok();
            }

            return BadRequest("Unable to delete photo.");
        }
    }
}
