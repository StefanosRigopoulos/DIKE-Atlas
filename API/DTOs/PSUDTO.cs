namespace API.DTOs
{
    public class PSUDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public int Wattage { get; set; } = 0;
        public string Certification { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}