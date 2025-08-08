using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IRAMRepository
    {
        void Update(RAM ram);
        void Delete(RAM ram);
        Task<bool> CheckUniquenessAsync(RAM ram);
        Task AddRAMAsync(RAM ram);
        Task<PagedList<RAMDTO>> GetRAMsWithParametersAsync(RAMParams ramParams);
        Task<RAM?> GetRAMByIDAsync(int id);
        Task<RAMDTO?> GetRAMDTOByIDAsync(int id);
        Task<IEnumerable<RAM>> GetRAMsAsync();
        Task<IEnumerable<RAMDTO>> GetRAMsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}