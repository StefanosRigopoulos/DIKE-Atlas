using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class RAMRepository(DataContext context, IMapper mapper) : IRAMRepository
    {
        public void Update(RAM ram)
        {
            context.Entry(ram).State = EntityState.Modified;
        }
        public void Delete(RAM ram)
        {
            context.Remove(ram);
        }
        public async Task<bool> CheckUniquenessAsync(RAM ram) {
            return await context.RAMs.AnyAsync(x => x.SerialNumber == ram.SerialNumber);
        }
        public async Task AddRAMAsync(RAM ram)
        {
            await context.RAMs.AddAsync(ram);
        }
        public async Task<PagedList<RAMDTO>> GetRAMsWithParametersAsync(RAMParams ramParams)
        {
            var query = context.RAMs.AsQueryable();

            // Apply filtering based on RAMParams.
            if (!string.IsNullOrEmpty(ramParams.Brand))
                query = query.Where(ram => ram.Brand.Contains(ramParams.Brand));
            if (!string.IsNullOrEmpty(ramParams.Model))
                query = query.Where(ram => ram.Model.Contains(ramParams.Model));
            if (!string.IsNullOrEmpty(ramParams.Type))
                query = query.Where(ram => ram.Type.Contains(ramParams.Type));
            if (!string.IsNullOrEmpty(ramParams.Size))
                query = query.Where(ram => ram.Size.Contains(ramParams.Size));

            // Apply sorting.
            query = ramParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "type" => query.OrderBy(u => u.Type),
                "sizeMost" => query.OrderByDescending(u => u.Size),
                "sizeLeast" => query.OrderBy(u => u.Size),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<RAMDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<RAMDTO>(mapper.ConfigurationProvider),
                ramParams.PageNumber,
                ramParams.PageSize);
            return pagedList;
        }
        public async Task<RAM?> GetRAMByIDAsync(int id)
        {
            return await context.RAMs
                                .Include(x => x.PC_RAMs)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<RAMDTO?> GetRAMDTOByIDAsync(int id)
        {
            return await context.RAMs
                                .Include(x => x.PC_RAMs)
                                .ProjectTo<RAMDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<RAM>> GetRAMsAsync()
        {
            return await context.RAMs
                                .Include(x => x.PC_RAMs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<RAMDTO>> GetRAMsDTOAsync()
        {
            return await context.RAMs
                                .Include(x => x.PC_RAMs)
                                .ProjectTo<RAMDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.RAMs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.RAMs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}