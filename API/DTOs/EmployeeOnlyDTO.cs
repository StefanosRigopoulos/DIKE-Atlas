namespace API.DTOs
{
    public class EmployeeOnlyDTO
    {
        public int ID { get; set; }
        public string AM { get; set; } = "";
        public string Rank { get; set; } = "";
        public string Speciality { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Office { get; set; } = "";
        public string Position { get; set; } = "";
        public string PCUsername { get; set; } = "";
        public string SHDAEDUsername { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhotoPath { get; set; } = "";
    }
}