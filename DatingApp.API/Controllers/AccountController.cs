﻿using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Repositories.Interfaces;
using DatingApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IUserRepository userRepository;

        public AccountController(DataContext context, 
                                 ITokenService tokenService,
                                 IUserRepository userRepository)
        {
            this.context = context;
            this.tokenService = tokenService;
            this.userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.Username))
            {
                return BadRequest("User already exists.");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDTO.Username.Trim().ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDTO()
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userRepository.GetUserByUsernameAsync(loginDTO.Username);

            if(user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            //todo lenght of hashes should also be same
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                if(hashedPassword[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid username or password");
                }
            }

            return new UserDTO()
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url
            };
        }

        #region Private methods

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(user => user.UserName == username.Trim().ToLower());
        }

        #endregion Private methods
    }
}
