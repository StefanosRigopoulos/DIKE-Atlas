using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCRAMRepository(DataContext context, IMapper mapper) : IPCRAMRepository
    {
        public void Delete(PC_RAM pc_ram)
        {
            context.Remove(pc_ram);
        }
        public async Task AddPCRAMAsync(PC_RAM pc_ram)
        {
            await context.PC_RAMs.AddAsync(pc_ram);
        }
        public async Task<PC_RAM?> GetRelationshipAsync(int ramid, int pcid)
        {
            return await context.PC_RAMs
                                .FirstOrDefaultAsync(x => x.RAMID == ramid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_RAM>> GetRelationshipRAMsAsync(int ramid)
        {
            return await context.PC_RAMs
                                .Where(x => x.RAMID == ramid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_RAM>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_RAMs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_RAM>> GetRelationshipsAsync()
        {
            return await context.PC_RAMs
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int ramid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_RAMs.Any(pc_ram => pc_ram.RAMID == ramid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<RAM>> GetRAMsAsync(int pcid)
        {
            return await context.RAMs
                                .Where(ram => ram.PC_RAMs.Any(pc_ram => pc_ram.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int ramid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_RAMs.Any(pc_ram => pc_ram.RAMID == ramid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<RAMDTO>> GetRAMsDTOAsync(int pcid)
        {
            return await context.RAMs
                                .Where(ram => ram.PC_RAMs.Any(pc_ram => pc_ram.PCID == pcid))
                                .ProjectTo<RAMDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}