using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IStorageRepository
    {
        void Update(Storage storage);
        void Delete(Storage storage);
        Task<bool> CheckUniquenessAsync(Storage storage);
        Task AddStorageAsync(Storage storage);
        Task<PagedList<StorageDTO>> GetStoragesWithParametersAsync(StorageParams storageParams);
        Task<Storage?> GetStorageByIDAsync(int id);
        Task<StorageDTO?> GetStorageDTOByIDAsync(int id);
        Task<IEnumerable<Storage>> GetStoragesAsync();
        Task<IEnumerable<StorageDTO>> GetStoragesDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
        Task<List<string>> GetFilterType();
        Task<List<string>> GetFilterInterface();
    }
}