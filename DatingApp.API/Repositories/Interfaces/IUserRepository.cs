using DatingApp.API.DTOs;
using DatingApp.API.Entities;
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

        Task<IEnumerable<MemberDTO>> GetMembersAsync();

        Task<MemberDTO> GetMembersByUsernameAsync(string username);
    }
}
