using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly ILogger<UsersController> logger;
        private readonly DataContext db;

        public UsersController(ILogger<UsersController> logger, DataContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            //logger.LogInformation($"Thread id {Thread.CurrentThread.ManagedThreadId}.");
            return await db.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await db.Users.FindAsync(id);
        }
    }
}
