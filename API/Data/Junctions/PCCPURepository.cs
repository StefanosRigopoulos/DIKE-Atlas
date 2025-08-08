using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCCPURepository(DataContext context, IMapper mapper) : IPCCPURepository
    {
        public void Delete(PC_CPU pc_cpu)
        {
            context.Remove(pc_cpu);
        }
        public async Task AddPCCPUAsync(PC_CPU pc_cpu)
        {
            await context.PC_CPUs.AddAsync(pc_cpu);
        }
        public async Task<PC_CPU?> GetRelationshipAsync(int cpuid, int pcid)
        {
            return await context.PC_CPUs
                                .FirstOrDefaultAsync(x => x.CPUID == cpuid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_CPU>> GetRelationshipCPUsAsync(int cpuid)
        {
            return await context.PC_CPUs
                                .Where(x => x.CPUID == cpuid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_CPU>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_CPUs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_CPU>> GetRelationshipsAsync()
        {
            return await context.PC_CPUs
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int cpuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_CPUs.Any(pc_cpu => pc_cpu.CPUID == cpuid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<CPU>> GetCPUsAsync(int pcid)
        {
            return await context.CPUs
                                .Where(cpu => cpu.PC_CPUs.Any(pc_cpu => pc_cpu.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int cpuid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_CPUs.Any(pc_cpu => pc_cpu.CPUID == cpuid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<CPUDTO>> GetCPUsDTOAsync(int pcid)
        {
            return await context.CPUs
                                .Where(cpu => cpu.PC_CPUs.Any(pc_cpu => pc_cpu.PCID == pcid))
                                .ProjectTo<CPUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}