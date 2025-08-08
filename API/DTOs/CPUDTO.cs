namespace API.DTOs
{
    public class CPUDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public int Cores { get; set; } = 0;
        public int Threads { get; set; } = 0;
        public string Specification { get; set; } = "";
        public string Package { get; set; } = "";
        public string Chipset { get; set; } = "";
        public string IntegratedGPUModel { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}