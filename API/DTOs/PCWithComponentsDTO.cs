namespace API.DTOs
{
    public class PCWithComponentsDTO
    {
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string PCName { get; set; } = "";
        public string AdministratorCode { get; set; } = "";
        public string BIOSCode { get; set; } = "";
        public string PDFReportPath { get; set; } = "";
        public string Domain { get; set; } = "";
        public string IP { get; set; } = "";
        public string ExternalIP { get; set; } = "";
        public string SubnetMask { get; set; } = "";
        public string Gateway { get; set; } = "";
        public string DNS1 { get; set; } = "";
        public string DNS2 { get; set; } = "";
        public List<CPUDTO> CPUs { get; set; } = [];
        public List<MOBODTO> MOBOs { get; set; } = [];
        public List<RAMDTO> RAMs { get; set; } = [];
        public List<GPUDTO> GPUs { get; set; } = [];
        public List<PSUDTO> PSUs { get; set; } = [];
        public List<StorageDTO> Storages { get; set; } = [];
        public List<NetworkCardDTO> NetworkCards { get; set; } = [];
        public List<MonitorDTO> Monitors { get; set; } = [];
    }
}