namespace API.DTOs
{
    public class UserDTO
    {
        public int ID { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}