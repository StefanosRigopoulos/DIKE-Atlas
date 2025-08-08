using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.DTOs;
using API.DTOs.User;
using API.Entities;
using API.Interfaces;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class AdminController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper, IUnitOfWork uow, IWebHostEnvironment env) : BaseAPIController
    {
        // User Management
        // Get User
        [HttpGet("user/get/{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id) {
            var user = await uow.UserRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new ApiException(404, "User Not Found", "No user exists with this ID."));
            return Ok(mapper.Map<UserDTO>(user));
        }
        // Get Users
        [HttpGet("user/get/all")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers() {
            var users = await uow.UserRepository.GetUsersDTOAsync();
            return Ok(users);
        }
        // Register User
        [HttpPost("user/register")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO registerDTO) {
            if (await UsernameExists(registerDTO.UserName)) return BadRequest(new ApiException(400, "Information duplicate", "User already present with this name!"));
            var user = mapper.Map<AppUser>(registerDTO);
            user.UserName = registerDTO.UserName.ToLower();
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var roleResult = await userManager.AddToRoleAsync(user, registerDTO.Role);
            if (!roleResult.Succeeded) return BadRequest(result.Errors);
            return new UserDTO
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = await tokenService.CreateToken(user),
                Role = user.Role.ToString()
            };
        }
        // Update User
        [HttpPut("user/update")]
        public async Task<ActionResult<UserDTO>> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var user = await uow.UserRepository.GetUserByIdAsync(updateUserDTO.ID);
            if (user == null) return NotFound(new ApiException(404, "User Not Found", "No user exists with this ID."));
            user.UserName = updateUserDTO.UserName?.ToLower() ?? user.UserName;
            user.FirstName = updateUserDTO.FirstName ?? user.FirstName;
            user.LastName = updateUserDTO.LastName ?? user.LastName;
            user.Role = updateUserDTO.Role ?? user.Role;
            if (!string.IsNullOrEmpty(updateUserDTO.NewPassword))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await userManager.ResetPasswordAsync(user, token, updateUserDTO.NewPassword);
                if (!passwordResult.Succeeded) return BadRequest(passwordResult.Errors);
            }
            var currentRoles = await userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(updateUserDTO.Role!))
            {
                await userManager.RemoveFromRolesAsync(user, currentRoles);
                var roleResult = await userManager.AddToRoleAsync(user, updateUserDTO.Role!);
                if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            }
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the User!"));
            return new UserDTO
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = await tokenService.CreateToken(user),
                Role = user.Role
            };
        }
        // Delete User
        [HttpDelete("user/delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await uow.UserRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new ApiException(404, "User Not Found", "No user exists with this ID."));
            if (user.Role == "Admin")
            {
                return BadRequest(new ApiException(400, "Action Not Allowed", "Admin users cannot be deleted."));
            }
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the User!"));
            return Ok(new { message = "User deleted successfully" });
        }
        private async Task<bool> UsernameExists(string username){
            return await userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        // Database Management
        // Export database.
        [HttpGet("database/export")]
        public async Task<IActionResult> ExportDatabase()
        {
            var stream = await uow.FileRepository.ExportDatabaseAsync();
            if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the database data!"));
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ΔΙΚΕ Βάση Δεδομένων.xlsx");
        }
        // Export table.
        [HttpGet("database/export/{tableName}")]
        public async Task<IActionResult> ExportTable(string tableName)
        {
            if (tableName.Equals("Employees")) {
                var stream = await uow.FileRepository.ExportEmployeeTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the Employee data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Στελέχοι.xlsx");
            } else if (tableName.Equals("PCs")) {
                var stream = await uow.FileRepository.ExportPCTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the PC data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Υπολογιστές.xlsx");
            } else if (tableName.Equals("CPUs")) {
                var stream = await uow.FileRepository.ExportCPUTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the CPU data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Επεξεργαστές.xlsx");
            } else if (tableName.Equals("MOBOs")) {
                var stream = await uow.FileRepository.ExportMOBOTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the MOBO data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Μητρικές Κάρτες.xlsx");
            } else if (tableName.Equals("RAMs")) {
                var stream = await uow.FileRepository.ExportRAMTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the RAM data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Μνήμες.xlsx");
            } else if (tableName.Equals("GPUs")) {
                var stream = await uow.FileRepository.ExportGPUTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the GPU data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Κάρτες Γραφικών.xlsx");
            } else if (tableName.Equals("PSUs")) {
                var stream = await uow.FileRepository.ExportPSUTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the PSU data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Τροφοδοτικά.xlsx");
            } else if (tableName.Equals("Storages")) {
                var stream = await uow.FileRepository.ExportStorageTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the Storage data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Αποθηκευτικά Μέσα.xlsx");
            } else if (tableName.Equals("NetworkCards")) {
                var stream = await uow.FileRepository.ExportNetworkCardTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the NetworkCard data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Κάρτες Δικτύου.xlsx");
            } else if (tableName.Equals("Monitors")) {
                var stream = await uow.FileRepository.ExportMonitorTableAsync();
                if (stream == null || stream.Length == 0) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to export the Monitor data!"));
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Οθόνες.xlsx");
            }
            return BadRequest(new ApiException(400, "Bad Request", "The table name is invalid!"));
        }

        // Backup
        // Import entire DB via JSON.
        [HttpPost("backup/import")]
        public async Task<IActionResult> ImportBackup([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new ApiException(400, "Bad Request", "No file uploaded!"));
            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var json = Encoding.UTF8.GetString(stream.ToArray());
                var data = JsonSerializer.Deserialize<DataModel>(json, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                if (data == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid JSON format!"));
                bool success = await uow.FileRepository.ImportDBAsync(data);
                if (!success) return BadRequest(new ApiException(400, "Bad Request", "Error while trying to import the data!"));
                return Ok(new { message = "Data imported successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiException(500, "Internal Server Error", $"Error processing file: {ex.Message}"));
            }
        }
        // Export entire DB in a JSON.
        [HttpGet("backup/export")]
        public async Task<IActionResult> ExportBackup()
        {
            var data = await uow.FileRepository.ExportDBAsync();
            if (data == null) return NotFound(new ApiException(404, "Not Found", "Data is empty!"));
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var fileBytes = Encoding.UTF8.GetBytes(json);
            return File(fileBytes, "application/json", "db_backup.json");
        }

        // Miscellaneous
        // Upload report backgorund image.
        [HttpPost("misc/background")]
        public async Task<IActionResult> UploadReportBackground([FromForm] IFormFile file) {
            if (file == null || file.Length == 0) return BadRequest(new ApiException(400, "Bad Request", "File not provided or empty!"));
            string folderPath = Path.Combine(env.ContentRootPath, "Storage");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "background.png");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { message = "File uploaded successfully!" });
        }
    }
}