using API.DTOs;
using API.Entities;
using API.Entities.Junctions;
using API.Helpers;
using API.Interfaces;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class EmployeeController(IUnitOfWork uow, IMapper mapper, IWebHostEnvironment env) : BaseAPIController
    {
        // Add a new Employee.
        [HttpPost("add")]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee([FromForm] EmployeeDTO employeeDTO, [FromForm] IFormFile? file) {
            if (employeeDTO == null)
                return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete Employee data!"));
            // Assign the DTO to an entity.
            var employee = mapper.Map<Employee>(employeeDTO);
            employee.PhotoPath= "";
            // Add the newly created PC to the database.
            await uow.EmployeeRepository.AddEmployeeAsync(employee);
            if (!await uow.Complete())
                return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the employee!"));
            // Handle file upload if a file is provided
            if (file != null)
            {
                string folderPath = Path.Combine(env.ContentRootPath, "Storage", "Employees", employee.ID.ToString());
                Directory.CreateDirectory(folderPath); // Ensure directory exists
                string filePath = Path.Combine(folderPath, "profile.jpg");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Save the file path to the database
                employee.PhotoPath = filePath;
            }
            // Update the database entity with the new data.
            uow.EmployeeRepository.Update(employee);
            if (await uow.Complete())
                return Ok(mapper.Map<EmployeeDTO>(employee));
            return BadRequest(new ApiException(400, "Bad Request", "Error while saving profile with image path!"));
        }
        // Update an existing Employee
        [HttpPut("update/{id}")]
        public async Task<ActionResult<EmployeeDTO>> UpdateEmployee(int id, [FromForm] EmployeeDTO employeeDTO, [FromForm] IFormFile? file) {
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(id);
            if (employee == null)
                return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            mapper.Map(employeeDTO, employee);
            if (file != null && file.Length > 0)
            {
                string folderPath = Path.Combine(env.ContentRootPath, "Storage", "Employees", id.ToString());
                Directory.CreateDirectory(folderPath);
                string filePath = Path.Combine(folderPath, "profile.jpg");
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                employee.PhotoPath = filePath;
            }
            uow.EmployeeRepository.Update(employee);
            if (await uow.Complete())
                return Ok(mapper.Map<EmployeeDTO>(employee));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the employee!"));
        }
        // Delete an existing Employee
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteEmployee(int id) {
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(id);
            if (employee == null)
                return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var employee_pcRelationships = await uow.EmployeePCRepository.GetRelationshipPCsAsync(id);
            foreach (Employee_PC relationship in employee_pcRelationships) {
                uow.EmployeePCRepository.Delete(relationship);
            }
            string folderPath = Path.Combine(env.ContentRootPath, "Storage", "Employees", id.ToString());
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, true);
            uow.EmployeeRepository.Delete(employee);
            if (await uow.Complete())
                return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the employee!"));
        }
        // GET Endpoints
        // Get specific employee based on ID
        [HttpGet("get/{id}")]
        public async Task<ActionResult<EmployeeDTO?>> GetEmployee(int id)
        {
            var employee = await uow.EmployeeRepository.GetEmployeeDTOByIDAsync(id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            return Ok(employee);
        }
        // Get specific employee based on ID
        [HttpGet("get/{id}/photo")]
        public async Task<IActionResult> GetProfileImage(int id)
        {
            var employee = await uow.EmployeeRepository.GetEmployeeDTOByIDAsync(id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            if (!System.IO.File.Exists(employee.PhotoPath) || string.IsNullOrEmpty(employee.PhotoPath))
                //return NotFound(new ApiException(404, "Not Found", "Profile image not found on disk!"));
                return NoContent();
            var imageBytes = System.IO.File.ReadAllBytes(employee.PhotoPath);
            return File(imageBytes, "image/png");
        }
        // Get all employees
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO?>>> GetEmployees()
        {
            var employees = await uow.EmployeeRepository.GetEmployeesDTOAsync();
            return Ok(employees);
        }
        // Get Employees with pagination.
        [HttpGet("get/all/paged")]
        public async Task<ActionResult<PagedList<EmployeeOnlyDTO>>> GetPagedEmployees([FromQuery]EmployeeParams employeeParams)
        {
            var pagedEmployees = await uow.EmployeeRepository.GetEmployeesWithParametersAsync(employeeParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedEmployees.CurrentPage, pagedEmployees.PageSize, pagedEmployees.TotalCount, pagedEmployees.TotalPages));
            return Ok(pagedEmployees);
        }
        
        // Relationship Endpoints
        // Assign Employee-PC relationship.
        [HttpPut("pc_assign/{employee_id}/{pc_id}")]
        public async Task<ActionResult<PCDTO>> AssignPC(int employee_id, int pc_id) {
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationship = await uow.EmployeePCRepository.GetRelationshipAsync(employee_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var employee_pc = new Employee_PC { EmployeeID = employee_id, PCID = pc_id };
            await uow.EmployeePCRepository.AddEmployeePCAsync(employee_pc);
            if (await uow.Complete()) return Ok(mapper.Map<PCDTO>(pc));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to assign PC to Employee!"));
        }
        // Get Related PC with the Employee given.
        [HttpGet("pc_get/{employee_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPCs(int employee_id)
        {
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var relationship = await uow.EmployeePCRepository.GetRelationshipEmployeesAsync(employee_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "Employee does not have PCs assigned to!"));
            var pcs = await uow.EmployeePCRepository.GetPCsDTOAsync(employee_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        // Cancel Employee-PC relationship.
        [HttpDelete("pc_remove/{employee_id}/{pc_id}")]
        public async Task<ActionResult> RemovePC(int employee_id, int pc_id) {
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationship = await uow.EmployeePCRepository.GetRelationshipAsync(employee_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the Employee and the PC!"));
            uow.EmployeePCRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to remove PC from Employee!"));
        }

        // Filter Options
        [HttpGet("filter/unit")]
        public async Task<ActionResult<List<string>>> FilterUnit() {
            return await uow.EmployeeRepository.GetFilterUnit();
        }
        [HttpGet("filter/office")]
        public async Task<ActionResult<List<string>>> FilterOffice() {
            return await uow.EmployeeRepository.GetFilterOffice();
        }
        [HttpGet("filter/rank")]
        public async Task<ActionResult<List<string>>> FilterRank() {
            return await uow.EmployeeRepository.GetFilterRank();
        }
        [HttpGet("filter/speciality")]
        public async Task<ActionResult<List<string>>> FilterSpeciality() {
            return await uow.EmployeeRepository.GetFilterSpeciality();
        }
    }
}