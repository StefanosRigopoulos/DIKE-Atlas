using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCPSURepository
    {
        void Delete(PC_PSU pc_psu);
        Task AddPCPSUAsync(PC_PSU pc_psu);
        Task<PC_PSU?> GetRelationshipAsync(int psuid, int pcid);
        Task<IEnumerable<PC_PSU>> GetRelationshipPSUsAsync(int psuid);
        Task<IEnumerable<PC_PSU>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_PSU>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int psuid);
        Task<IEnumerable<PSU>> GetPSUsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int psuid);
        Task<IEnumerable<PSUDTO>> GetPSUsDTOAsync(int pcid);
    }
}