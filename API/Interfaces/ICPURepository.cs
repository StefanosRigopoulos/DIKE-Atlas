using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ICPURepository
    {
        void Update(CPU cpu);
        void Delete(CPU cpu);
        Task<bool> CheckUniquenessAsync(CPU cpu);
        Task AddCPUAsync(CPU cpu);
        Task<PagedList<CPUDTO>> GetCPUsWithParametersAsync(CPUParams cpuParams);
        Task<CPU?> GetCPUByIDAsync(int id);
        Task<CPUDTO?> GetCPUDTOByIDAsync(int id);
        Task<IEnumerable<CPU>> GetCPUsAsync();
        Task<IEnumerable<CPUDTO>> GetCPUsDTOAsync();
        Task<IEnumerable<CPUDTO>> GetUnassignedCPUsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
        Task<List<string>> GetFilterCore();
    }
}