using API.DTOs;
using API.Entities;
using API.Entities.Junctions;

namespace API.Interfaces.Junctions
{
    public interface IEmployeePCRepository
    {
        void Delete(Employee_PC employee_pc);
        Task AddEmployeePCAsync(Employee_PC employee_pc);
        Task<Employee_PC?> GetRelationshipAsync(int employeeid, int pcid);
        Task<IEnumerable<Employee_PC>> GetRelationshipEmployeesAsync(int employeeid);
        Task<IEnumerable<Employee_PC>> GetRelationshipPCsAsync(int pcid);
        Task<IEnumerable<Employee_PC>> GetRelationshipsAsync();
        Task<IEnumerable<Employee>> GetEmployeesAsync(int pcid);
        Task<IEnumerable<PC>> GetPCsAsync(int employeeid);
        Task<IEnumerable<EmployeeDTO>> GetEmployeesDTOAsync(int pcid);
        Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int employeeid);
    }
}