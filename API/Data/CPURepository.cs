using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Data
{
    public class CPURepository(DataContext context, IMapper mapper) : ICPURepository
    {
        public void Update(CPU cpu)
        {
            context.Entry(cpu).State = EntityState.Modified;
        }
        public void Delete(CPU cpu)
        {
            context.Remove(cpu);
        }
        public async Task<bool> CheckUniquenessAsync(CPU cpu) {
            return await context.CPUs.AnyAsync(x => x.SerialNumber == cpu.SerialNumber);
        }
        public async Task AddCPUAsync(CPU cpu)
        {
            await context.CPUs.AddAsync(cpu);
        }
        public async Task<PagedList<CPUDTO>> GetCPUsWithParametersAsync(CPUParams cpuParams)
        {
            var query = context.CPUs.AsQueryable();

            // Apply filtering based on CPUParams.
            if (!string.IsNullOrEmpty(cpuParams.Barcode))
                query = query.Where(cpu => cpu.Barcode.Equals(cpuParams.Barcode));
            if (!string.IsNullOrEmpty(cpuParams.Brand))
                query = query.Where(cpu => cpu.Brand.Contains(cpuParams.Brand));
            if (!string.IsNullOrEmpty(cpuParams.Model))
                query = query.Where(cpu => cpu.Model.Contains(cpuParams.Model));
            int cores = 0;
            if (!string.IsNullOrEmpty(cpuParams.Cores) && int.TryParse(cpuParams.Cores, out cores))
                query = query.Where(cpu => cpu.Cores == cores);

            // Apply sorting.
            query = cpuParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "coresMost" => query.OrderByDescending(u => u.Cores),
                "coresLeast" => query.OrderBy(u => u.Cores),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<CPUDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<CPUDTO>(mapper.ConfigurationProvider),
                cpuParams.PageNumber,
                cpuParams.PageSize);
            return pagedList;
        }
        public async Task<CPU?> GetCPUByIDAsync(int id)
        {
            return await context.CPUs
                                .Include(x => x.PC_CPUs)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<CPUDTO?> GetCPUDTOByIDAsync(int id)
        {
            return await context.CPUs
                                .Include(x => x.PC_CPUs)
                                .ProjectTo<CPUDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<CPU>> GetCPUsAsync()
        {
            return await context.CPUs
                                .Include(x => x.PC_CPUs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<CPUDTO>> GetCPUsDTOAsync()
        {
            return await context.CPUs
                                .Include(x => x.PC_CPUs)
                                .ProjectTo<CPUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<CPUDTO>> GetUnassignedCPUsDTOAsync()
        {
            return await context.CPUs
                                .Where(x => x.PC_CPUs.Count == 0)
                                .ProjectTo<CPUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.CPUs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.CPUs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterCore() {
            return await context.CPUs
                                .Where(u => u.Cores != 0)
                                .Select(u => u.Cores.ToString())
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}