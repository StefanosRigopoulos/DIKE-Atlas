namespace API.DTOs
{
    public class PCDTO
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
        public List<int> EmployeeIDs { get; set; } = [];
        public List<int> CPUIDs { get; set; } = [];
        public List<int> MOBOIDs { get; set; } = [];
        public List<int> RAMIDs { get; set; } = [];
        public List<int> GPUIDs { get; set; } = [];
        public List<int> PSUIDs { get; set; } = [];
        public List<int> StorageIDs { get; set; } = [];
        public List<int> NetworkCardIDs { get; set; } = [];
        public List<int> MonitorIDs { get; set; } = [];
    }
}