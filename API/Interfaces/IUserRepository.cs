using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        void Delete(AppUser user);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserDTOByIdAsync(int id);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<IEnumerable<UserDTO>> GetUsersDTOAsync();
    }
}