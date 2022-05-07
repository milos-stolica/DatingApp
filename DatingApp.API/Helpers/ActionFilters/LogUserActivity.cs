using DatingApp.API.Extensions;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers.ActionFilters
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextAfterAction = await next();

            HttpContext httpContext = contextAfterAction.HttpContext;

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var userRepo = httpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;

            var userId = httpContext.User.GetUserId();
            var user = await userRepo.GetUserByIdAsync(userId);

            user.LastActive = DateTime.UtcNow;
            await userRepo.SaveAllChangesAsync();
        }
    }
}
