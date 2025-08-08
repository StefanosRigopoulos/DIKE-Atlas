namespace API.Entities.Junctions
{
    public class PC_Storage
    {
        // Foreign Keys
        public int PCID { get; set; } = 0;
        public int StorageID { get; set; } = 0;

        // Navigation Properties
        public PC PC { get; set; } = null!;
        public Storage Storage { get; set; } = null!;
    }
}