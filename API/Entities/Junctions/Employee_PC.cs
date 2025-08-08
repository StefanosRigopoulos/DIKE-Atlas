namespace API.Entities.Junctions
{
    public class Employee_PC
    {
        // Foreign Keys
        public int EmployeeID { get; set; } = 0;
        public int PCID { get; set; } = 0;

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
        public PC PC { get; set; } = null!;
    }
}