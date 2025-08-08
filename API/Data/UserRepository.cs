using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }
        public void Delete(AppUser user)
        {
            context.Remove(user);
        }
        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }
        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await context.Users
                                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<UserDTO?> GetUserDTOByIdAsync(int id)
        {
            return await context.Users
                                .ProjectTo<UserDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.Users
                                .ToListAsync();
        }
        public async Task<IEnumerable<UserDTO>> GetUsersDTOAsync()
        {
            return await context.Users
                                .ProjectTo<UserDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}