namespace API.Entities.Junctions
{
    public class PC_RAM
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int RAMID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public RAM RAM { get; set; } = null!;
    }
}