namespace API.Entities.Junctions
{
    public class PC_NetworkCard
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int NetworkCardID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public NetworkCard NetworkCard { get; set; } = null!;
    }
}