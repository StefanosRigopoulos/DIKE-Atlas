using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMOBORepository
    {
        void Update(MOBO mobo);
        void Delete(MOBO mobo);
        Task<bool> CheckUniquenessAsync(MOBO mobo);
        Task AddMOBOAsync(MOBO mobo);
        Task<PagedList<MOBODTO>> GetMOBOsWithParametersAsync(MOBOParams moboParams);
        Task<MOBO?> GetMOBOByIDAsync(int id);
        Task<MOBODTO?> GetMOBODTOByIDAsync(int id);
        Task<IEnumerable<MOBO>> GetMOBOsAsync();
        Task<IEnumerable<MOBODTO>> GetMOBOsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}