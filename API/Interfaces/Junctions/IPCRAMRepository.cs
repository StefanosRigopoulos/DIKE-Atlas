using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCRAMRepository
    {
        void Delete(PC_RAM pc_ram);
        Task AddPCRAMAsync(PC_RAM pc_ram);
        Task<PC_RAM?> GetRelationshipAsync(int ramid, int pcid);
        Task<IEnumerable<PC_RAM>> GetRelationshipRAMsAsync(int ramid);
        Task<IEnumerable<PC_RAM>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_RAM>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int ramid);
        Task<IEnumerable<RAM>> GetRAMsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int ramid);
        Task<IEnumerable<RAMDTO>> GetRAMsDTOAsync(int pcid);
    }
}