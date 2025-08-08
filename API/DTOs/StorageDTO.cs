namespace API.DTOs
{
    public class StorageDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Type { get; set; } = "";
        public string Interface { get; set; } = "";
        public string Speed { get; set; } = "";
        public string Capacity { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}