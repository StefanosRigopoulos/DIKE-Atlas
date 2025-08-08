using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCMOBORepository(DataContext context, IMapper mapper) : IPCMOBORepository
    {
        public void Delete(PC_MOBO pc_mobo)
        {
            context.Remove(pc_mobo);
        }
        public async Task AddPCMOBOAsync(PC_MOBO pc_mobo)
        {
            await context.PC_MOBOs.AddAsync(pc_mobo);
        }
        public async Task<PC_MOBO?> GetRelationshipAsync(int moboid, int pcid)
        {
            return await context.PC_MOBOs
                                .FirstOrDefaultAsync(x => x.MOBOID == moboid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_MOBO>> GetRelationshipMOBOsAsync(int moboid)
        {
            return await context.PC_MOBOs
                                .Where(x => x.MOBOID == moboid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_MOBO>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_MOBOs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_MOBO>> GetRelationshipsAsync()
        {
            return await context.PC_MOBOs
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int moboid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_MOBOs.Any(pc_mobo => pc_mobo.MOBOID == moboid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<MOBO>> GetMOBOsAsync(int pcid)
        {
            return await context.MOBOs
                                .Where(mobo => mobo.PC_MOBOs.Any(pc_mobo => pc_mobo.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int moboid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_MOBOs.Any(pc_mobo => pc_mobo.MOBOID == moboid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<MOBODTO>> GetMOBOsDTOAsync(int pcid)
        {
            return await context.MOBOs
                                .Where(mobo => mobo.PC_MOBOs.Any(pc_mobo => pc_mobo.PCID == pcid))
                                .ProjectTo<MOBODTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}