using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Repositories.Interfaces;
using DatingApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ITokenService tokenService,
                                 IUserRepository userRepository,
                                 IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.Username))
            {
                return BadRequest("User already exists.");
            }

            var user = mapper.Map<AppUser>(registerDTO);

            user.UserName = user.UserName.Trim().ToLower();

            IdentityResult result = await userManager.CreateAsync(user, registerDTO.Password);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await userManager.AddToRoleAsync(user, "Member");

            if(!result.Succeeded)
            {
                //todo if this happens than user shoudl be deleted from database (should be done in one transaction)
                return BadRequest(roleResult.Errors);
            }

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userRepository.GetUserByUsernameAsync(loginDTO.Username);

            if(user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!result.Succeeded)
            {
                return Unauthorized("Invalid username or password.");
            }

            return new UserDTO()
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        #region Private methods

        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(user => user.UserName == username.Trim().ToLower());
        }

        #endregion Private methods
    }
}
