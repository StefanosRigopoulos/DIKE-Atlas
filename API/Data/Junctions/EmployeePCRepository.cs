using API.DTOs;
using API.Entities;
using API.Entities.Junctions;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Junctions
{
    public class EmployeePCRepository(DataContext context, IMapper mapper) : IEmployeePCRepository
    {
        public void Delete(Employee_PC employee_pc)
        {
            context.Remove(employee_pc);
        }
        public async Task AddEmployeePCAsync(Employee_PC employee_pc)
        {
            await context.Employee_PCs.AddAsync(employee_pc);
        }
        public async Task<Employee_PC?> GetRelationshipAsync(int employeeid, int pcid)
        {
            return await context.Employee_PCs
                                .FirstOrDefaultAsync(x => x.EmployeeID == employeeid && x.PCID == pcid);
        }
        public async Task<IEnumerable<Employee_PC>> GetRelationshipEmployeesAsync(int employeeid)
        {
            return await context.Employee_PCs
                                .Where(x => x.EmployeeID == employeeid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<Employee_PC>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.Employee_PCs
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<Employee_PC>> GetRelationshipsAsync()
        {
            return await context.Employee_PCs
                                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(int pcid)
        {
            return await context.Employees
                                .Where(employee => employee.Employee_PCs.Any(employee_pc => employee_pc.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int employeeid)
        {
            return await context.PCs
                                .Where(pc => pc.Employee_PCs.Any(employee_pc => employee_pc.EmployeeID == employeeid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesDTOAsync(int pcid)
        {
            return await context.Employees
                                .Where(employee => employee.Employee_PCs.Any(employee_pc => employee_pc.PCID == pcid))
                                .ProjectTo<EmployeeDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int employeeid)
        {
            return await context.PCs
                                .Where(pc => pc.Employee_PCs.Any(employee_pc => employee_pc.EmployeeID == employeeid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}