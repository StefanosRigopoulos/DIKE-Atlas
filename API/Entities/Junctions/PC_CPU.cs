namespace API.Entities.Junctions
{
    public class PC_CPU
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int CPUID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public CPU CPU { get; set; } = null!;
    }
}