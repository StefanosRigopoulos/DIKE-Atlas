using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCPSURepository(DataContext context, IMapper mapper) : IPCPSURepository
    {
        public void Delete(PC_PSU pc_psu)
        {
            context.Remove(pc_psu);
        }
        public async Task AddPCPSUAsync(PC_PSU pc_psu)
        {
            await context.PC_PSUs.AddAsync(pc_psu);
        }
        public async Task<PC_PSU?> GetRelationshipAsync(int psuid, int pcid)
        {
            return await context.PC_PSUs
                                .FirstOrDefaultAsync(x => x.PSUID == psuid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_PSU>> GetRelationshipPSUsAsync(int psuid)
        {
            return await context.PC_PSUs
                                .Where(x => x.PSUID == psuid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_PSU>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_PSUs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_PSU>> GetRelationshipsAsync()
        {
            return await context.PC_PSUs
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int psuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_PSUs.Any(pc_psu => pc_psu.PSUID == psuid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PSU>> GetPSUsAsync(int pcid)
        {
            return await context.PSUs
                                .Where(psu => psu.PC_PSUs.Any(pc_psu => pc_psu.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int psuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_PSUs.Any(pc_psu => pc_psu.PSUID == psuid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PSUDTO>> GetPSUsDTOAsync(int pcid)
        {
            return await context.PSUs
                                .Where(psu => psu.PC_PSUs.Any(pc_psu => pc_psu.PCID == pcid))
                                .ProjectTo<PSUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}