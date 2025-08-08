using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MonitorRepository(DataContext context, IMapper mapper) : IMonitorRepository
    {
        public void Update(Entities.Monitor monitor)
        {
            context.Entry(monitor).State = EntityState.Modified;
        }
        public void Delete(Entities.Monitor monitor)
        {
            context.Remove(monitor);
        }
        public async Task<bool> CheckUniquenessAsync(Entities.Monitor monitor) {
            return await context.Monitors.AnyAsync(x => x.SerialNumber == monitor.SerialNumber);
        }
        public async Task AddMonitorAsync(Entities.Monitor monitor)
        {
            await context.Monitors.AddAsync(monitor);
        }
        public async Task<PagedList<MonitorDTO>> GetMonitorsWithParametersAsync(MonitorParams monitorParams)
        {
            var query = context.Monitors.AsQueryable();

            // Apply filtering based on MonitorParams.
            if (!string.IsNullOrEmpty(monitorParams.Brand))
                query = query.Where(monitor => monitor.Brand.Contains(monitorParams.Brand));
            if (!string.IsNullOrEmpty(monitorParams.Model))
                query = query.Where(monitor => monitor.Model.Contains(monitorParams.Model));
            if (!string.IsNullOrEmpty(monitorParams.Resolution))
                query = query.Where(monitor => monitor.Resolution.Contains(monitorParams.Resolution));
            int inches = 0;
            if (!string.IsNullOrEmpty(monitorParams.Inches) && int.TryParse(monitorParams.Inches, out inches))
                query = query.Where(monitor => monitor.Inches == inches);

            // Apply sorting.
            query = monitorParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "resolution" => query.OrderBy(u => u.Resolution),
                "inchesMost" => query.OrderByDescending(u => u.Inches),
                "inchesLeast" => query.OrderBy(u => u.Inches),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<MonitorDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<MonitorDTO>(mapper.ConfigurationProvider),
                monitorParams.PageNumber,
                monitorParams.PageSize);
            return pagedList;
        }
        public async Task<Entities.Monitor?> GetMonitorByIDAsync(int id)
        {
            return await context.Monitors
                                .Include(x => x.PC_Monitors)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<MonitorDTO?> GetMonitorDTOByIDAsync(int id)
        {
            return await context.Monitors
                                .Include(x => x.PC_Monitors)
                                .ProjectTo<MonitorDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<Entities.Monitor>> GetMonitorsAsync()
        {
            return await context.Monitors
                                .Include(x => x.PC_Monitors)
                                .ToListAsync();
        }
        public async Task<IEnumerable<MonitorDTO>> GetMonitorsDTOAsync()
        {
            return await context.Monitors
                                .Include(x => x.PC_Monitors)
                                .ProjectTo<MonitorDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.Monitors
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.Monitors
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}