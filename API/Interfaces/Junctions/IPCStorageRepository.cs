using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCStorageRepository
    {
        void Delete(PC_Storage pc_storage);
        Task AddPCStorageAsync(PC_Storage pc_storage);
        Task<PC_Storage?> GetRelationshipAsync(int storageid, int pcid);
        Task<IEnumerable<PC_Storage>> GetRelationshipStoragesAsync(int storageid);
        Task<IEnumerable<PC_Storage>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_Storage>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int storageid);
        Task<IEnumerable<Storage>> GetStoragesAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int storageid);
        Task<IEnumerable<StorageDTO>> GetStoragesDTOAsync(int pcid);
    }
}