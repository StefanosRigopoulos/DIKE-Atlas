namespace API.Helpers
{
    public class EmployeeParams : PaginationParams
    {
        public string? Rank { get; set; }
        public string? Speciality { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Unit { get; set; }
        public string? Office { get; set; }
        public string OrderBy { get; set; } = "lastname";
    }
}