using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class GPURepository(DataContext context, IMapper mapper) : IGPURepository
    {
        public void Update(GPU gpu)
        {
            context.Entry(gpu).State = EntityState.Modified;
        }
        public void Delete(GPU gpu)
        {
            context.Remove(gpu);
        }
        public async Task<bool> CheckUniquenessAsync(GPU gpu) {
            return await context.GPUs.AnyAsync(x => x.SerialNumber == gpu.SerialNumber);
        }
        public async Task AddGPUAsync(GPU gpu)
        {
            await context.GPUs.AddAsync(gpu);
        }
        public async Task<PagedList<GPUDTO>> GetGPUsWithParametersAsync(GPUParams gpuParams)
        {
            var query = context.GPUs.AsQueryable();

            // Apply filtering based on GPUParams.
            if (!string.IsNullOrEmpty(gpuParams.Brand))
                query = query.Where(gpu => gpu.Brand.Contains(gpuParams.Brand));
            if (!string.IsNullOrEmpty(gpuParams.Model))
                query = query.Where(gpu => gpu.Model.Contains(gpuParams.Model));
            if (!string.IsNullOrEmpty(gpuParams.Memory))
                query = query.Where(gpu => gpu.Memory.Contains(gpuParams.Memory));

            // Apply sorting.
            query = gpuParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "memoryMost" => query.OrderByDescending(u => u.Memory),
                "memoryLeast" => query.OrderBy(u => u.Memory),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<GPUDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<GPUDTO>(mapper.ConfigurationProvider),
                gpuParams.PageNumber,
                gpuParams.PageSize);
            return pagedList;
        }
        public async Task<GPU?> GetGPUByIDAsync(int id)
        {
            return await context.GPUs
                                .Include(x => x.PC_GPUs)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<GPUDTO?> GetGPUDTOByIDAsync(int id)
        {
            return await context.GPUs
                                .Include(x => x.PC_GPUs)
                                .ProjectTo<GPUDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<GPU>> GetGPUsAsync()
        {
            return await context.GPUs
                                .Include(x => x.PC_GPUs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<GPUDTO>> GetGPUsDTOAsync()
        {
            return await context.GPUs
                                .Include(x => x.PC_GPUs)
                                .ProjectTo<GPUDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.GPUs
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.GPUs
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}