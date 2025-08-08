using API.DTOs;
using API.Entities.Junctions;
using API.Entities;

namespace API.Interfaces.Junctions
{
    public interface IPCNetworkCardRepository
    {
        void Delete(PC_NetworkCard pc_networkcard);
        Task AddPCNetworkCardAsync(PC_NetworkCard pc_networkcard);
        Task<PC_NetworkCard?> GetRelationshipAsync(int networkcardid, int pcid);
        Task<IEnumerable<PC_NetworkCard>> GetRelationshipNetworkCardsAsync(int networkcardid);
        Task<IEnumerable<PC_NetworkCard>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<PC_NetworkCard>> GetRelationshipsAsync();
        Task<IEnumerable<PC>> GetPCsAsync(int networkcardid);
        Task<IEnumerable<NetworkCard>> GetNetworkCardsAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int networkcardid);
        Task<IEnumerable<NetworkCardDTO>> GetNetworkCardsDTOAsync(int pcid);
    }
}