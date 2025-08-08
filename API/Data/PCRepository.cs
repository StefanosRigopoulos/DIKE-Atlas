using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCRepository(DataContext context, IMapper mapper) : IPCRepository
    {
        public void Update(PC pc)
        {
            context.Entry(pc).State = EntityState.Modified;
        }
        public void Delete(PC pc)
        {
            context.Remove(pc);
        }
        public async Task<bool> CheckUniquenessAsync(PC pc) {
            return await context.PCs.AnyAsync(x => x.SerialNumber == pc.SerialNumber);
        }
        public async Task AddPCAsync(PC pc)
        {
            await context.PCs.AddAsync(pc);
        }
        public async Task<PagedList<PCOnlyDTO>> GetPCsWithParametersAsync(PCParams pcParams)
        {
            var query = context.PCs.AsQueryable();

            // Apply filtering based on PCParams.
            if (!string.IsNullOrEmpty(pcParams.Barcode))
                query = query.Where(pc => pc.Barcode.Equals(pcParams.Barcode));
            if (!string.IsNullOrEmpty(pcParams.Brand))
                query = query.Where(pc => pc.Brand.Contains(pcParams.Brand));
            if (!string.IsNullOrEmpty(pcParams.Model))
                query = query.Where(pc => pc.Model.Contains(pcParams.Model));
            if (!string.IsNullOrEmpty(pcParams.Domain))
                query = query.Where(pc => pc.Domain.Contains(pcParams.Domain));

            // Apply sorting.
            query = pcParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<PCOnlyDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<PCOnlyDTO>(mapper.ConfigurationProvider),
                pcParams.PageNumber,
                pcParams.PageSize);
            return pagedList;
        }
        public async Task<PC?> GetPCByIDAsync(int ID)
        {
            return await context.PCs
                                .Include(x => x.PC_CPUs)
                                .Include(x => x.PC_MOBOs)
                                .Include(x => x.PC_RAMs)
                                .Include(x => x.PC_GPUs)
                                .Include(x => x.PC_PSUs)
                                .Include(x => x.PC_Storages)
                                .Include(x => x.PC_NetworkCards)
                                .Include(x => x.PC_Monitors)
                                .FirstOrDefaultAsync(x => x.ID == ID);
        }
        public async Task<PCDTO?> GetPCDTOByIDAsync(int ID)
        {
            return await context.PCs
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == ID);
        }
        public async Task<PCWithComponentsDTO?> GetPCWithComponentsDTOByIDAsync(int ID)
        {
            return await context.PCs
                                .Include(pc => pc.Employee_PCs).ThenInclude(employee_pc => employee_pc.Employee)
                                .Include(pc => pc.PC_CPUs).ThenInclude(pc_cpu => pc_cpu.CPU)
                                .Include(pc => pc.PC_MOBOs).ThenInclude(pc_mobo => pc_mobo.MOBO)
                                .Include(pc => pc.PC_RAMs).ThenInclude(pc_ram => pc_ram.RAM)
                                .Include(pc => pc.PC_GPUs).ThenInclude(pc_gpu => pc_gpu.GPU)
                                .Include(pc => pc.PC_PSUs).ThenInclude(pc_psu => pc_psu.PSU)
                                .Include(pc => pc.PC_Storages).ThenInclude(pc_storage => pc_storage.Storage)
                                .Include(pc => pc.PC_NetworkCards).ThenInclude(pc_networkCard => pc_networkCard.NetworkCard)
                                .Include(pc => pc.PC_Monitors).ThenInclude(pc_monitor => pc_monitor.Monitor)
                                .ProjectTo<PCWithComponentsDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(pc => pc.ID == ID);
        }
        public async Task<IEnumerable<PC>> GetPCsAsync()
        {
            return await context.PCs
                                .Include(x => x.PC_CPUs)
                                .Include(x => x.PC_MOBOs)
                                .Include(x => x.PC_RAMs)
                                .Include(x => x.PC_GPUs)
                                .Include(x => x.PC_PSUs)
                                .Include(x => x.PC_Storages)
                                .Include(x => x.PC_NetworkCards)
                                .Include(x => x.PC_Monitors)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCOnlyDTO>> GetPCsDTOAsync()
        {
            return await context.PCs
                                .ProjectTo<PCOnlyDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.PCs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.PCs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterDomain() {
            return await context.PCs
                                .Where(u => u.Domain != null && u.Domain != "")
                                .Select(u => u.Domain)
                                .Distinct()
                                .ToListAsync();
        }
    }
}