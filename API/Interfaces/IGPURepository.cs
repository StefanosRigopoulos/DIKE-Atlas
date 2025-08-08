using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IGPURepository
    {
        void Update(GPU gpu);
        void Delete(GPU gpu);
        Task<bool> CheckUniquenessAsync(GPU gpu);
        Task AddGPUAsync(GPU gpu);
        Task<PagedList<GPUDTO>> GetGPUsWithParametersAsync(GPUParams gpuParams);
        Task<GPU?> GetGPUByIDAsync(int id);
        Task<GPUDTO?> GetGPUDTOByIDAsync(int id);
        Task<IEnumerable<GPU>> GetGPUsAsync();
        Task<IEnumerable<GPUDTO>> GetGPUsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}