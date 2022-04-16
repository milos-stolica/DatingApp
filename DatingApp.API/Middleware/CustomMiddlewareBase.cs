using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Middleware
{
    public abstract class CustomMiddlewareBase
    {
        protected readonly RequestDelegate next;
        protected readonly ILogger<CustomMiddlewareBase> logger;
        protected readonly IHostEnvironment env;

        public CustomMiddlewareBase(RequestDelegate next, ILogger<CustomMiddlewareBase> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public abstract Task InvokeAsync(HttpContext context);
    }
}
