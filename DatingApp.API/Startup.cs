using DatingApp.API.Extensions;
using DatingApp.API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(configuration)
                    .AddIdentityServices(configuration)
                    .AddCors()
                    .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            string allowedOrigin = "https://localhost:4200";
            app.UseCors(policy => 
                            policy.AllowAnyHeader().
                                   AllowAnyMethod().
                                   WithOrigins(allowedOrigin));

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
