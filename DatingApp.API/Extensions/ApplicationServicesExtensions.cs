﻿using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Repositories;
using DatingApp.API.Repositories.Interfaces;
using DatingApp.API.Services;
using DatingApp.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DatingApp.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
                                                                IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"))
                       .LogTo(Console.WriteLine)
                       .EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
