using API.DTOs;

namespace API.Helpers
{
    public class DataModel
    {
        public List<EmployeeDTO> Employees { get; set; } = new();
        public List<PCDTO> PCs { get; set; } = new();
        public List<CPUDTO> CPUs { get; set; } = new();
        public List<MOBODTO> MOBOs { get; set; } = new();
        public List<RAMDTO> RAMs { get; set; } = new();
        public List<GPUDTO> GPUs { get; set; } = new();
        public List<PSUDTO> PSUs { get; set; } = new();
        public List<StorageDTO> Storages { get; set; } = new();
        public List<NetworkCardDTO> NetworkCards { get; set; } = new();
        public List<MonitorDTO> Monitors { get; set; } = new();
    }
}