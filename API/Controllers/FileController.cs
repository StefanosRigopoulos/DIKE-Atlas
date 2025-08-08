using API.Helpers;
using API.Interfaces;
using API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace API.Controllers
{
    [Authorize]
    public class FileController(IUnitOfWork uow) : BaseAPIController
    {
        // Import table.
        [HttpPost("import/{tableName}")]
        public async Task<IActionResult> ImportTable(string tableName, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new ApiException(400, "Bad Request", "No file uploaded!"));
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var rows = worksheet.Dimension.Rows;
            if (rows > 1) {
                bool result = false;
                if (tableName.Equals("Employees")) result = await uow.FileRepository.ImportEmployeeTableAsync(worksheet, rows);
                else if (tableName.Equals("PCs")) result = await uow.FileRepository.ImportPCTableAsync(worksheet, rows);
                else if (tableName.Equals("CPUs")) result = await uow.FileRepository.ImportCPUTableAsync(worksheet, rows);
                else if (tableName.Equals("MOBOs")) result = await uow.FileRepository.ImportMOBOTableAsync(worksheet, rows);
                else if (tableName.Equals("RAMs")) result = await uow.FileRepository.ImportRAMTableAsync(worksheet, rows);
                else if (tableName.Equals("GPUs")) result = await uow.FileRepository.ImportGPUTableAsync(worksheet, rows);
                else if (tableName.Equals("PSUs")) result = await uow.FileRepository.ImportPSUTableAsync(worksheet, rows);
                else if (tableName.Equals("Storages")) result = await uow.FileRepository.ImportStorageTableAsync(worksheet, rows);
                else if (tableName.Equals("NetworkCards")) result = await uow.FileRepository.ImportNetworkCardTableAsync(worksheet, rows);
                else if (tableName.Equals("Monitors")) result = await uow.FileRepository.ImportMonitorTableAsync(worksheet, rows);
                if (result) {
                    if (await uow.Complete()) return Ok("Data imported successfully!");
                    return BadRequest(new ApiException(400, "Bad Request", "Error while trying to import the data!"));
                } else {
                    return Ok("We have no changes to apply!");
                }
            }
            return BadRequest(new ApiException(400, "Bad Request", "The import file was empty!"));
        }
        // Get our own PC Report.
        [HttpGet("get/report/{pc_id}")]
        public async Task<IActionResult> GetPCReport(int pc_id) {
            var pc = await uow.PCRepository.GetPCWithComponentsDTOByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var pdfBytes = ReportGenerator.GeneratePDF(pc);
            if (pdfBytes.IsNullOrEmpty()) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to create report file!"));
            return File(pdfBytes, "application/pdf", "Report_" + pc.Barcode + ".pdf");
        }
        // Get Employee Card.
        [HttpGet("get/card/{employee_id}")]
        public async Task<IActionResult> GetEmployeeCard(int employee_id) {
            var employee = await uow.EmployeeRepository.GetEmployeeDTOByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var cardBytes = ReportGenerator.GenerateEmployeeCard(employee);
            if (cardBytes.IsNullOrEmpty()) return BadRequest(new ApiException(500, "Internal Server Error", "Error while trying to create report file!"));
            return File(cardBytes, "application/pdf", "Card_" + employee.FirstName + "_" + employee.LastName + ".pdf");
        }
    }
}