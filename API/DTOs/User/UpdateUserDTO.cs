namespace API.DTOs.User
{
    public class UpdateUserDTO
    {
        public required int ID { get; set; }
        public string? UserName { get; set; }
        public string? NewPassword { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
    }
}