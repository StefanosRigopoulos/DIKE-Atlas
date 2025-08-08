namespace API.Entities.Junctions
{
    public class PC_MOBO
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int MOBOID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public MOBO MOBO { get; set; } = null!;
    }
}