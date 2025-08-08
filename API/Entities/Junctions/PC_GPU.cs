namespace API.Entities.Junctions
{
    public class PC_GPU
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int GPUID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public GPU GPU { get; set; } = null!;
    }
}