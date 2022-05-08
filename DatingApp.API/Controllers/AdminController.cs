using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users
                                   //.Include(user => user.Roles) - same result with or without these two lines
                                   //.ThenInclude(role => role.Role)
                                   .OrderBy(user => user.UserName)
                                   .Select(user => new { 
                                       Id = user.Id,
                                       Username = user.UserName,
                                       Roles = user.Roles.Select(role => role.Role.Name)
                                   })
                                   .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult<IEnumerable<string>>> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",");

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var rolesToAdd = selectedRoles.Except(userRoles);
            var rolesToDelete = userRoles.Except(selectedRoles);

            var changeRolesResult = await userManager.AddToRolesAsync(user, rolesToAdd);

            //todo this should be transaction

            if (!changeRolesResult.Succeeded)
            {
                return BadRequest("Problem adding roles");
            }

            changeRolesResult = await userManager.RemoveFromRolesAsync(user, rolesToDelete);

            if (!changeRolesResult.Succeeded)
            {
                return BadRequest("Problem removing roles");
            }

            return Ok(await userManager.GetRolesAsync(user));
        }


        [Authorize(Policy = "RequireModerateRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosToModerate()
        {
            return Ok("Admins and moderators can see this.");
        }
    }
}
