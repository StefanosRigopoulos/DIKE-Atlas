namespace API.DTOs
{
    public class MonitorDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Resolution { get; set; } = "";
        public int Inches { get; set; }
        public List<int> PCIDs { get; set; } = [];
    }
}