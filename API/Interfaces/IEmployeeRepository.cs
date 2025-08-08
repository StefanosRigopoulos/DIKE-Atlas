using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IEmployeeRepository
    {
        void Update(Employee employee);
        void Delete(Employee employee);
        Task AddEmployeeAsync(Employee employee);
        Task<PagedList<EmployeeOnlyDTO>> GetEmployeesWithParametersAsync(EmployeeParams pcParams);
        Task<Employee?> GetEmployeeByIDAsync(int id);
        Task<EmployeeDTO?> GetEmployeeDTOByIDAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<EmployeeDTO>> GetEmployeesDTOAsync();
        Task<List<string>> GetFilterUnit();
        Task<List<string>> GetFilterOffice();
        Task<List<string>> GetFilterRank();
        Task<List<string>> GetFilterSpeciality();
    }
}