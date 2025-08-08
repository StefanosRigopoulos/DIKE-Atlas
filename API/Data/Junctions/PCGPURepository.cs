using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCGPURepository(DataContext context, IMapper mapper) : IPCGPURepository
    {
        public void Delete(PC_GPU pc_gpu)
        {
            context.Remove(pc_gpu);
        }
        public async Task AddPCGPUAsync(PC_GPU pc_gpu)
        {
            await context.PC_GPUs.AddAsync(pc_gpu);
        }
        public async Task<PC_GPU?> GetRelationshipAsync(int gpuid, int pcid)
        {
            return await context.PC_GPUs
                                .FirstOrDefaultAsync(x => x.GPUID == gpuid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_GPU>> GetRelationshipGPUsAsync(int gpuid)
        {
            return await context.PC_GPUs
                                .Where(x => x.GPUID == gpuid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_GPU>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_GPUs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_GPU>> GetRelationshipsAsync()
        {
            return await context.PC_GPUs
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int gpuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_GPUs.Any(pc_gpu => pc_gpu.GPUID == gpuid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<GPU>> GetGPUsAsync(int pcid)
        {
            return await context.GPUs
                                .Where(gpu => gpu.PC_GPUs.Any(pc_gpu => pc_gpu.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int gpuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_GPUs.Any(pc_gpu => pc_gpu.GPUID == gpuid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<GPUDTO>> GetGPUsDTOAsync(int pcid)
        {
            return await context.GPUs
                                .Where(gpu => gpu.PC_GPUs.Any(pc_gpu => pc_gpu.PCID == pcid))
                                .ProjectTo<GPUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}