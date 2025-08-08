using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCMonitorRepository
    {
        void Delete(PC_Monitor pc_monitor);
        Task AddPCMonitorAsync(PC_Monitor pc_monitor);
        Task<PC_Monitor?> GetRelationshipAsync(int monitorid, int pcid);
        Task<IEnumerable<PC_Monitor>> GetRelationshipMonitorsAsync(int monitorid);
        Task<IEnumerable<PC_Monitor>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_Monitor>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int monitorid);
        Task<IEnumerable<Entities.Monitor>> GetMonitorsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int monitorid);
        Task<IEnumerable<MonitorDTO>> GetMonitorsDTOAsync(int pcid);
    }
}