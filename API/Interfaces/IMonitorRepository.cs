using API.DTOs;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMonitorRepository
    {
        void Update(Entities.Monitor monitor);
        void Delete(Entities.Monitor monitor);
        Task<bool> CheckUniquenessAsync(Entities.Monitor monitor);
        Task AddMonitorAsync(Entities.Monitor monitor);
        Task<PagedList<MonitorDTO>> GetMonitorsWithParametersAsync(MonitorParams monitorParams);
        Task<Entities.Monitor?> GetMonitorByIDAsync(int id);
        Task<MonitorDTO?> GetMonitorDTOByIDAsync(int id);
        Task<IEnumerable<Entities.Monitor>> GetMonitorsAsync();
        Task<IEnumerable<MonitorDTO>> GetMonitorsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}