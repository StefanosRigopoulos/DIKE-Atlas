using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PSURepository(DataContext context, IMapper mapper) : IPSURepository
    {
        public void Update(PSU psu)
        {
            context.Entry(psu).State = EntityState.Modified;
        }
        public void Delete(PSU psu)
        {
            context.Remove(psu);
        }
        public async Task<bool> CheckUniquenessAsync(PSU psu) {
            return await context.PSUs.AnyAsync(x => x.SerialNumber == psu.SerialNumber);
        }
        public async Task AddPSUAsync(PSU psu)
        {
            await context.PSUs.AddAsync(psu);
        }
        public async Task<PagedList<PSUDTO>> GetPSUsWithParametersAsync(PSUParams psuParams)
        {
            var query = context.PSUs.AsQueryable();

            // Apply filtering based on PSUParams
            if (!string.IsNullOrEmpty(psuParams.Brand))
                query = query.Where(psu => psu.Brand.Contains(psuParams.Brand));
            if (!string.IsNullOrEmpty(psuParams.Model))
                query = query.Where(psu => psu.Model.Contains(psuParams.Model));
            if (psuParams.Wattage > 0)
                query = query.Where(psu => psu.Wattage == psuParams.Wattage);

            query = psuParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "wattageMost" => query.OrderByDescending(u => u.Wattage),
                "wattageLeast" => query.OrderBy(u => u.Wattage),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<PSUDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<PSUDTO>(mapper.ConfigurationProvider),
                psuParams.PageNumber,
                psuParams.PageSize);
            return pagedList;
        }
        public async Task<PSU?> GetPSUByIDAsync(int id)
        {
            return await context.PSUs
                                .Include(x => x.PC_PSUs)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<PSUDTO?> GetPSUDTOByIDAsync(int id)
        {
            return await context.PSUs
                                .Include(x => x.PC_PSUs)
                                .ProjectTo<PSUDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<PSU>> GetPSUsAsync()
        {
            return await context.PSUs
                                .Include(x => x.PC_PSUs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PSUDTO>> GetPSUsDTOAsync()
        {
            return await context.PSUs
                                .Include(x => x.PC_PSUs)
                                .ProjectTo<PSUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.PSUs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.PSUs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}