using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public UserRepository(DataContext dataContext,
                              IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            IQueryable<MemberDTO> query = dataContext.Users
                                            .Where(user => user.UserName != userParams.CurrentUsername)
                                            .Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob)
                                            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                                            .AsNoTracking();

            if(userParams.Gender != null)
            {
                query = query.Where(user => user.Gender == userParams.Gender);
            }

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(user => user.Created),
                _ => query.OrderByDescending(user => user.LastActive)
            };

            return await PagedList<MemberDTO>.CreatePagedList(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDTO> GetMembersByUsernameAsync(string username)
        {
            return await dataContext.Users
                .Where(user => user.UserName == username)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await dataContext.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await dataContext.Users
                .Include(user => user.Photos)
                .SingleOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await dataContext.Users
                .Include(user => user.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            dataContext.Entry(user).State = EntityState.Modified;
        }
    }
}
