using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCMOBORepository
    {
        void Delete(PC_MOBO pc_mobo);
        Task AddPCMOBOAsync(PC_MOBO pc_mobo);
        Task<PC_MOBO?> GetRelationshipAsync(int moboid, int pcid);
        Task<IEnumerable<PC_MOBO>> GetRelationshipMOBOsAsync(int moboid);
        Task<IEnumerable<PC_MOBO>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_MOBO>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int moboid);
        Task<IEnumerable<MOBO>> GetMOBOsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int moboid);
        Task<IEnumerable<MOBODTO>> GetMOBOsDTOAsync(int pcid);
    }
}