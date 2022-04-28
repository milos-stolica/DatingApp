using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Helpers.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllChangesAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);

        Task<MemberDTO> GetMembersByUsernameAsync(string username);
    }
}
