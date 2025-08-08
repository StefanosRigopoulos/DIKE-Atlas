namespace API.Entities.Junctions
{
    public class PC_PSU
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int PSUID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public PSU PSU { get; set; } = null!;
    }
}