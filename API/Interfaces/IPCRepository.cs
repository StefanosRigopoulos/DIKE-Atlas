using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IPCRepository
    {
        void Update(PC pc);
        void Delete(PC pc);
        Task<bool> CheckUniquenessAsync(PC pc);
        Task AddPCAsync(PC pc);
        Task<PagedList<PCOnlyDTO>> GetPCsWithParametersAsync(PCParams pcParams);
        Task<PC?> GetPCByIDAsync(int id);
        Task<PCDTO?> GetPCDTOByIDAsync(int id);
        Task<PCWithComponentsDTO?> GetPCWithComponentsDTOByIDAsync(int id);
        Task<IEnumerable<PC>> GetPCsAsync();
        Task<IEnumerable<PCOnlyDTO>> GetPCsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
        Task<List<string>> GetFilterDomain();
    }
}