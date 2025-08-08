using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MOBORepository(DataContext context, IMapper mapper) : IMOBORepository
    {
        public void Update(MOBO mobo)
        {
            context.Entry(mobo).State = EntityState.Modified;
        }
        public void Delete(MOBO mobo)
        {
            context.Remove(mobo);
        }
        public async Task<bool> CheckUniquenessAsync(MOBO mobo) {
            return await context.MOBOs.AnyAsync(x => x.SerialNumber == mobo.SerialNumber);
        }
        public async Task AddMOBOAsync(MOBO mobo)
        {
            await context.MOBOs.AddAsync(mobo);
        }
        public async Task<PagedList<MOBODTO>> GetMOBOsWithParametersAsync(MOBOParams moboParams)
        {
            var query = context.MOBOs.AsQueryable();

            // Apply filtering based on MOBOParams.
            if (!string.IsNullOrEmpty(moboParams.Brand))
                query = query.Where(mobo => mobo.Brand.Contains(moboParams.Brand));
            if (!string.IsNullOrEmpty(moboParams.Model))
                query = query.Where(mobo => mobo.Model == moboParams.Model);
            int dramslots = 0;
            if (!string.IsNullOrEmpty(moboParams.DRAMSlots) && int.TryParse(moboParams.DRAMSlots, out dramslots))
                query = query.Where(mobo => mobo.DRAMSlots == dramslots);
            if (!string.IsNullOrEmpty(moboParams.ChipsetModel))
                query = query.Where(mobo => mobo.ChipsetModel.Contains(moboParams.ChipsetModel));

            // Apply sorting.
            query = moboParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "chipsetModel" => query.OrderBy(u => u.ChipsetModel),
                "dramSlotsMost" => query.OrderByDescending(u => u.DRAMSlots),
                "dramSlotsLeast" => query.OrderBy(u => u.DRAMSlots),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<MOBODTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<MOBODTO>(mapper.ConfigurationProvider),
                moboParams.PageNumber,
                moboParams.PageSize);
            return pagedList;
        }
        public async Task<MOBO?> GetMOBOByIDAsync(int id)
        {
            return await context.MOBOs
                                .Include(x => x.PC_MOBOs)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<MOBODTO?> GetMOBODTOByIDAsync(int id)
        {
            return await context.MOBOs
                                .Include(x => x.PC_MOBOs)
                                .ProjectTo<MOBODTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<MOBO>> GetMOBOsAsync()
        {
            return await context.MOBOs
                                .Include(x => x.PC_MOBOs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<MOBODTO>> GetMOBOsDTOAsync()
        {
            return await context.MOBOs
                                .Include(x => x.PC_MOBOs)
                                .ProjectTo<MOBODTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.MOBOs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.MOBOs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}