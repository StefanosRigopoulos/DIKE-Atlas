using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class EmployeeRepository(DataContext context, IMapper mapper) : IEmployeeRepository
    {
        public void Update(Employee employee)
        {
            context.Entry(employee).State = EntityState.Modified;
        }
        public void Delete(Employee employee)
        {
            context.Remove(employee);
        }
        public async Task AddEmployeeAsync(Employee employee)
        {
            await context.Employees.AddAsync(employee);
        }
        public async Task<PagedList<EmployeeOnlyDTO>> GetEmployeesWithParametersAsync(EmployeeParams pcParams)
        {
            var query = context.Employees.AsQueryable();

            // Apply filtering based on PCParams.
            if (!string.IsNullOrEmpty(pcParams.Rank))
                query = query.Where(pc => pc.Rank.Equals(pcParams.Rank));
            if (!string.IsNullOrEmpty(pcParams.Speciality))
                query = query.Where(pc => pc.Speciality.Equals(pcParams.Speciality));
            if (!string.IsNullOrEmpty(pcParams.FirstName))
                query = query.Where(pc => pc.FirstName.Equals(pcParams.FirstName));
            if (!string.IsNullOrEmpty(pcParams.LastName))
                query = query.Where(pc => pc.LastName.Equals(pcParams.LastName));
            if (!string.IsNullOrEmpty(pcParams.Unit))
                query = query.Where(pc => pc.Unit.Equals(pcParams.Unit));
            if (!string.IsNullOrEmpty(pcParams.Office))
                query = query.Where(pc => pc.Office.Equals(pcParams.Office));

            // Apply sorting.
            query = pcParams.OrderBy switch
            {
                "rank" => query.OrderBy(u => u.Rank),
                "speciality" => query.OrderBy(u => u.Speciality),
                "firstname" => query.OrderBy(u => u.FirstName),
                "lastname" => query.OrderBy(u => u.LastName),
                "unit" => query.OrderBy(u => u.Unit),
                "office" => query.OrderBy(u => u.Office),
                _ => query.OrderBy(u => u.LastName)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<EmployeeOnlyDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<EmployeeOnlyDTO>(mapper.ConfigurationProvider),
                pcParams.PageNumber,
                pcParams.PageSize);
            return pagedList;
        }
        public async Task<Employee?> GetEmployeeByIDAsync(int ID)
        {
            return await context.Employees
                                .Include(x => x.Employee_PCs)
                                .FirstOrDefaultAsync(x => x.ID == ID);
        }
        public async Task<EmployeeDTO?> GetEmployeeDTOByIDAsync(int ID)
        {
            return await context.Employees
                                .Include(x => x.Employee_PCs)
                                .ProjectTo<EmployeeDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == ID);
        }
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await context.Employees
                                .Include(x => x.Employee_PCs)
                                .ToListAsync();
        }
        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesDTOAsync()
        {
            return await context.Employees
                                .Include(x => x.Employee_PCs)
                                .ProjectTo<EmployeeDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterUnit() {
            return await context.Employees
                                .Where(u => u.Unit != null && u.Unit != "")
                                .Select(u => u.Unit)
                                .Distinct()
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterOffice() {
            return await context.Employees
                                .Where(u => u.Office != null && u.Office != "")
                                .Select(u => u.Office)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterRank() {
            return await context.Employees
                                .Where(u => u.Rank != null && u.Rank != "")
                                .Select(u => u.Rank)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterSpeciality() {
            return await context.Employees
                                .Where(u => u.Speciality != null && u.Speciality != "")
                                .Select(u => u.Speciality)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}