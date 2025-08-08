using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class StorageRepository(DataContext context, IMapper mapper) : IStorageRepository
    {
        public void Update(Storage storage)
        {
            context.Entry(storage).State = EntityState.Modified;
        }
        public void Delete(Storage storage)
        {
            context.Remove(storage);
        }
        public async Task<bool> CheckUniquenessAsync(Storage storage) {
            return await context.Storages.AnyAsync(x => x.SerialNumber == storage.SerialNumber);
        }
        public async Task AddStorageAsync(Storage storage)
        {
            await context.Storages.AddAsync(storage);
        }
        public async Task<PagedList<StorageDTO>> GetStoragesWithParametersAsync(StorageParams storageParams)
        {
            var query = context.Storages.AsQueryable();

            // Apply filtering based on StorageParams.
            if (!string.IsNullOrEmpty(storageParams.Brand))
                query = query.Where(storage => storage.Brand.Contains(storageParams.Brand));
            if (!string.IsNullOrEmpty(storageParams.Model))
                query = query.Where(storage => storage.Model.Contains(storageParams.Model));
            if (!string.IsNullOrEmpty(storageParams.Type))
                query = query.Where(storage => storage.Type.Contains(storageParams.Type));
            if (!string.IsNullOrEmpty(storageParams.Capacity))
                query = query.Where(storage => storage.Capacity.Contains(storageParams.Capacity));

            // Apply sorting.
            query = storageParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                "type" => query.OrderBy(u => u.Type),
                "capacityMost" => query.OrderByDescending(u => u.Capacity),
                "capacityLeast" => query.OrderBy(u => u.Capacity),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<StorageDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<StorageDTO>(mapper.ConfigurationProvider),
                storageParams.PageNumber,
                storageParams.PageSize);
            return pagedList;
        }
        public async Task<Storage?> GetStorageByIDAsync(int id)
        {
            return await context.Storages
                                .Include(x => x.PC_Storages)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<StorageDTO?> GetStorageDTOByIDAsync(int id)
        {
            return await context.Storages
                                .Include(x => x.PC_Storages)
                                .ProjectTo<StorageDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<Storage>> GetStoragesAsync()
        {
            return await context.Storages
                                .Include(x => x.PC_Storages)
                                .ToListAsync();
        }
        public async Task<IEnumerable<StorageDTO>> GetStoragesDTOAsync()
        {
            return await context.Storages
                                .Include(x => x.PC_Storages)
                                .ProjectTo<StorageDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.Storages
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.Storages
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterType() {
            return await context.Storages
                                .Where(u => u.Type != null && u.Type != "")
                                .Select(u => u.Type)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterInterface() {
            return await context.Storages
                                .Where(u => u.Interface != null && u.Interface != "")
                                .Select(u => u.Interface)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}