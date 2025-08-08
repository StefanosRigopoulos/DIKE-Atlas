using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IPSURepository
    {
        void Update(PSU psu);
        void Delete(PSU psu);
        Task<bool> CheckUniquenessAsync(PSU psu);
        Task AddPSUAsync(PSU psu);
        Task<PagedList<PSUDTO>> GetPSUsWithParametersAsync(PSUParams psuParams);
        Task<PSU?> GetPSUByIDAsync(int id);
        Task<PSUDTO?> GetPSUDTOByIDAsync(int id);
        Task<IEnumerable<PSU>> GetPSUsAsync();
        Task<IEnumerable<PSUDTO>> GetPSUsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}