namespace API.DTOs
{
    public class GPUDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Memory { get; set; } = "";
        public string DriverVersion { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}