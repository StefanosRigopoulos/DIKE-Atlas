namespace API.DTOs
{
    public class NetworkCardDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}