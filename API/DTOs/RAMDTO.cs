namespace API.DTOs
{
    public class RAMDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string CASLatency { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}