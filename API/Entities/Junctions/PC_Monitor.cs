namespace API.Entities.Junctions
{
    public class PC_Monitor
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int MonitorID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public Entities.Monitor Monitor { get; set; } = null!;
    }
}