using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCGPURepository
    {
        void Delete(PC_GPU pc_gpu);
        Task AddPCGPUAsync(PC_GPU pc_gpu);
        Task<PC_GPU?> GetRelationshipAsync(int gpuid, int pcid);
        Task<IEnumerable<PC_GPU>> GetRelationshipGPUsAsync(int gpuid);
        Task<IEnumerable<PC_GPU>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_GPU>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int gpuid);
        Task<IEnumerable<GPU>> GetGPUsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int gpuid);
        Task<IEnumerable<GPUDTO>> GetGPUsDTOAsync(int pcid);
    }
}