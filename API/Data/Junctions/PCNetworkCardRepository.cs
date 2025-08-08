using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCNetworkCardRepository(DataContext context, IMapper mapper) : IPCNetworkCardRepository
    {
        public void Delete(PC_NetworkCard pc_networkcard)
        {
            context.Remove(pc_networkcard);
        }
        public async Task AddPCNetworkCardAsync(PC_NetworkCard pc_networkcard)
        {
            await context.PC_NetworkCards.AddAsync(pc_networkcard);
        }
        public async Task<PC_NetworkCard?> GetRelationshipAsync(int networkcardid, int pcid)
        {
            return await context.PC_NetworkCards
                                .FirstOrDefaultAsync(x => x.NetworkCardID == networkcardid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_NetworkCard>> GetRelationshipNetworkCardsAsync(int networkcardid)
        {
            return await context.PC_NetworkCards
                                .Where(x => x.NetworkCardID == networkcardid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_NetworkCard>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_NetworkCards
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_NetworkCard>> GetRelationshipsAsync()
        {
            return await context.PC_NetworkCards
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int networkcardid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_NetworkCards.Any(pc_networkcard => pc_networkcard.NetworkCardID == networkcardid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<NetworkCard>> GetNetworkCardsAsync(int pcid)
        {
            return await context.NetworkCards
                                .Where(networkcard => networkcard.PC_NetworkCards.Any(pc_networkcard => pc_networkcard.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int networkcardid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_NetworkCards.Any(pc_networkcard => pc_networkcard.NetworkCardID == networkcardid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<NetworkCardDTO>> GetNetworkCardsDTOAsync(int pcid)
        {
            return await context.NetworkCards
                                .Where(networkcard => networkcard.PC_NetworkCards.Any(pc_networkcard => pc_networkcard.PCID == pcid))
                                .ProjectTo<NetworkCardDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}