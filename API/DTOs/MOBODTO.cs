namespace API.DTOs
{
    public class MOBODTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public int DRAMSlots { get; set; }
        public string ChipsetVendor { get; set; } = "";
        public string ChipsetModel { get; set; } = "";
        public string BIOSBrand { get; set; } = "";
        public string BIOSVersion { get; set; } = "";
        public string OnBoardGPUBrand { get; set; } = "";
        public string OnBoardGPUModel { get; set; } = "";
        public string OnBoardNetworkAdapterBrand { get; set; } = "";
        public string OnBoardNetworkAdapterModel { get; set; } = "";
        public List<int> PCIDs { get; set; } = [];
    }
}