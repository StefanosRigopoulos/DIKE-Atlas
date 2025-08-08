using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCCPURepository
    {
        void Delete(PC_CPU pc_cpu);
        Task AddPCCPUAsync(PC_CPU pc_cpu);
        Task<PC_CPU?> GetRelationshipAsync(int cpuid, int pcid);
        Task<IEnumerable<PC_CPU>> GetRelationshipCPUsAsync(int cpuid);
        Task<IEnumerable<PC_CPU>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_CPU>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int cpuid);
        Task<IEnumerable<CPU>> GetCPUsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int cpuid);
        Task<IEnumerable<CPUDTO>> GetCPUsDTOAsync(int pcid);
    }
}