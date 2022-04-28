using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.API.Entities;
using DatingApp.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey key;
        public TokenService(IConfiguration config)
        {
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenSecret"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                //this one is needed for one request, so you dont have to query against database, but it is sent through token every time
                new Claim(JwtRegisteredClaimNames.Gender, string.IsNullOrEmpty(user.Gender) ? "" : user.Gender)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
