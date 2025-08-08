using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseAPIController
    {
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO) {
            var user = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
            if (user == null) return Unauthorized(new ApiException(401, "Invalid Username!", "Please try again!"));
            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result) return Unauthorized(new ApiException(401, "Invalid Password!", "Please try again!"));
            return new UserDTO
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = await tokenService.CreateToken(user),
                Role = user.Role.ToString()
            };
        }
    }
}