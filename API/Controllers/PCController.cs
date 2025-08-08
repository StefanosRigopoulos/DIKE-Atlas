using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class PCController(IUnitOfWork uow, IMapper mapper, IWebHostEnvironment env) : BaseAPIController
    {
        // Add
        [HttpPost("add")]
        public async Task<ActionResult<PCDTO>> AddPC([FromForm] PCDTO pcDTO, [FromForm] IFormFile? reportpdf) {
            if (pcDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete PC data!"));
            var pc = mapper.Map<PC>(pcDTO);
            if (pcDTO.SerialNumber != "")
                if (await uow.PCRepository.CheckUniquenessAsync(pc))
                    return BadRequest(new ApiException(400, "Bad Request", "PC is already registered!"));
            pc.Barcode = BarcodeGenerator.GenerateBarcode("PC");
            pc.PDFReportPath = "";
            await uow.PCRepository.AddPCAsync(pc);
            if (!await uow.Complete()) return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the PC!"));
            string folderPath = Path.Combine(env.ContentRootPath, "Storage", "PCs", pc.ID.ToString());
            Directory.CreateDirectory(folderPath);
            if (reportpdf != null)
            {
                string reportPath = Path.Combine(folderPath, "speccy_report.pdf");
                using (var stream = new FileStream(reportPath, FileMode.Create))
                {
                    await reportpdf.CopyToAsync(stream);
                }
                pc.PDFReportPath = reportPath;
            }
            uow.PCRepository.Update(pc);
            if (await uow.Complete()) return Ok(mapper.Map<PCDTO>(pc));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the PC!"));
        }
        // Update
        [HttpPut("update/{pc_id}")]
        public async Task<ActionResult<PCDTO>> UpdatePC(int pc_id, [FromForm] PCDTO pcDTO, [FromForm] IFormFile? reportpdf) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            mapper.Map(pcDTO, pc);
            pc.PDFReportPath = pcDTO.PDFReportPath;
            string folderPath = Path.Combine(env.ContentRootPath, "Storage", "PCs", pc_id.ToString());
            Directory.CreateDirectory(folderPath);
            if (reportpdf != null)
            {
                string reportPath = Path.Combine(folderPath, "speccy_report.pdf");
                if (System.IO.File.Exists(reportPath)) System.IO.File.Delete(reportPath);
                using (var stream = new FileStream(reportPath, FileMode.Create))
                {
                    await reportpdf.CopyToAsync(stream);
                }
                pc.PDFReportPath = reportPath;
            }
            uow.PCRepository.Update(pc);
            if (await uow.Complete()) return Ok(mapper.Map<PCDTO>(pc));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the PC!"));
        }
        // Delete
        [HttpDelete("delete/{pc_id}")]
        public async Task<ActionResult> DeletePC(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var pc_cpuRelationships = await uow.PCCPURepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_CPU relationship in pc_cpuRelationships) uow.PCCPURepository.Delete(relationship);
            var pc_moboRelationships = await uow.PCMOBORepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_MOBO relationship in pc_moboRelationships) uow.PCMOBORepository.Delete(relationship);
            var pc_ramRelationships = await uow.PCRAMRepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_RAM relationship in pc_ramRelationships) uow.PCRAMRepository.Delete(relationship);
            var pc_gpuRelationships = await uow.PCGPURepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_GPU relationship in pc_gpuRelationships) uow.PCGPURepository.Delete(relationship);
            var pc_psuRelationships = await uow.PCPSURepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_PSU relationship in pc_psuRelationships) uow.PCPSURepository.Delete(relationship);
            var pc_storageRelationships = await uow.PCStorageRepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_Storage relationship in pc_storageRelationships) uow.PCStorageRepository.Delete(relationship);
            var pc_networkcardRelationships = await uow.PCNetworkCardRepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_NetworkCard relationship in pc_networkcardRelationships) uow.PCNetworkCardRepository.Delete(relationship);
            var pc_monitorRelationships = await uow.PCMonitorRepository.GetRelationshipPCsAsync(pc_id);
            foreach (PC_Monitor relationship in pc_monitorRelationships) uow.PCMonitorRepository.Delete(relationship);
            string folderPath = Path.Combine(env.ContentRootPath, "Storage", "PCs", pc_id.ToString());
            if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
            uow.PCRepository.Delete(pc);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the PC!"));
        }

        // GET Endpoints
        // Get specific PC based on ID
        [HttpGet("get/{pc_id}")]
        public async Task<ActionResult<PCDTO?>> GetPC(int pc_id)
        {
            var pc = await uow.PCRepository.GetPCDTOByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            return Ok(pc);
        }
        // Get PCWithComponents by ID.
        [HttpGet("get/{pc_id}/components")]
        public async Task<ActionResult<PCWithComponentsDTO?>> GetPCWithComponents(int pc_id) {
            var pc = await uow.PCRepository.GetPCWithComponentsDTOByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            return Ok(pc);
        }
        // Get ReportPDF by ID.
        [HttpGet("get/{pc_id}/report")]
        public ActionResult GetReportPDF(int pc_id) {
            string folderPath = Path.Combine(env.ContentRootPath, "Storage", "PCs", pc_id.ToString());
            string filePath = Path.Combine(folderPath, "speccy_report.pdf");
            if (!System.IO.File.Exists(filePath)) return NotFound(new ApiException(404, "Not Found", "Requested PDF file not found!"));
            var pdfBytes = System.IO.File.ReadAllBytes(filePath);
            return File(pdfBytes, "application/pdf", "speccy_report.pdf");
        }
        // Get all PCs
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<PCOnlyDTO?>>> GetPCs()
        {
            var pcs = await uow.PCRepository.GetPCsDTOAsync();
            return Ok(pcs);
        }
        // Get PCs with pagination.
        [HttpGet("get/all/paged")]
        public async Task<ActionResult<PagedList<PCOnlyDTO>>> GetPagedPCs([FromQuery] PCParams pcParams)
        {
            var pagedPCs = await uow.PCRepository.GetPCsWithParametersAsync(pcParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedPCs.CurrentPage, pagedPCs.PageSize, pagedPCs.TotalCount, pagedPCs.TotalPages));
            return Ok(pagedPCs);
        }

        // Relationships Endpoint
        // Assign PC-Employee relationship.
        [HttpPut("employee_assign/{pc_id}/{employee_id}")]
        public async Task<ActionResult<EmployeeDTO>> AssignEmployee(int pc_id, int employee_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var relationship = await uow.EmployeePCRepository.GetRelationshipAsync(employee_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_employee = new Employee_PC { PCID = pc_id, EmployeeID = employee_id };
            await uow.EmployeePCRepository.AddEmployeePCAsync(pc_employee);
            if (await uow.Complete()) return Ok(mapper.Map<EmployeeDTO>(employee));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the PC!"));
        }
        // Get PC-Employee relationships.
        [HttpGet("employee_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetRelatedEmployees(int pc_id)
        {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.EmployeePCRepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have an Employee assigned to!"));
            var employees = await uow.EmployeePCRepository.GetEmployeesDTOAsync(pc_id);
            if (employees == null) return NotFound(new ApiException(404, "Not Found", "Related Employees not found!"));
            return Ok(employees);
        }
        // Cancel PC-Employee relationship.
        [HttpDelete("employee_remove/{pc_id}/{employee_id}")]
        public async Task<ActionResult> RemoveEmployee(int pc_id, int employee_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var employee = await uow.EmployeeRepository.GetEmployeeByIDAsync(employee_id);
            if (employee == null) return NotFound(new ApiException(404, "Not Found", "Employee not found!"));
            var relationship = await uow.EmployeePCRepository.GetRelationshipAsync(employee_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the Employee!"));
            uow.EmployeePCRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to remove remove Employee from PC!"));
        }
        // Assign PC-CPU relationship.
        [HttpPut("cpu_assign/{pc_id}/{cpu_id}")]
        public async Task<ActionResult<CPUDTO>> AssignCPU(int pc_id, int cpu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var cpu = await uow.CPURepository.GetCPUByIDAsync(cpu_id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            var relationship = await uow.PCCPURepository.GetRelationshipAsync(cpu_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_cpu = new PC_CPU { PCID = pc_id, CPUID = cpu_id };
            await uow.PCCPURepository.AddPCCPUAsync(pc_cpu);
            if (await uow.Complete()) return Ok(mapper.Map<CPUDTO>(cpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the CPU!"));
        }
        // Get PC-CPU relationships.
        [HttpGet("cpu_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<CPUDTO>>> GetRelatedCPUs(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCCPURepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have CPUs assigned to!"));
            var cpus = await uow.PCCPURepository.GetCPUsDTOAsync(pc_id);
            if (cpus == null) return NotFound(new ApiException(404, "Not Found", "Related CPUs not found!"));
            return Ok(mapper.Map<CPUDTO>(cpus));
        }
        // Cancel PC-CPU relationship.
        [HttpDelete("cpu_remove/{pc_id}/{cpu_id}")]
        public async Task<ActionResult> RemoveCPU(int pc_id, int cpu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var cpu = await uow.CPURepository.GetCPUByIDAsync(cpu_id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            var relationship = await uow.PCCPURepository.GetRelationshipAsync(cpu_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the CPU!"));
            uow.PCCPURepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to remove remove CPU from PC!"));
        }
        // Assign PC-MOBO relationship.
        [HttpPut("mobo_assign/{pc_id}/{mobo_id}")]
        public async Task<ActionResult<MOBODTO>> AssignMOBO(int pc_id, int mobo_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(mobo_id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            var relationship = await uow.PCMOBORepository.GetRelationshipAsync(mobo_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_mobo = new PC_MOBO { PCID = pc_id, MOBOID = mobo_id };
            await uow.PCMOBORepository.AddPCMOBOAsync(pc_mobo);
            if (await uow.Complete()) return Ok(mapper.Map<MOBODTO>(mobo));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the MOBO!"));
        }
        // Get PC-MOBO relationships.
        [HttpGet("mobo_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<MOBODTO>>> GetRelatedMOBOs(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCMOBORepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have MOBOs assigned to!"));
            var mobos = await uow.PCMOBORepository.GetMOBOsDTOAsync(pc_id);
            if (mobos == null) return NotFound(new ApiException(404, "Not Found", "Related MOBOs not found!"));
            return Ok(mapper.Map<MOBODTO>(mobos));
        }
        // Cancel PC-MOBO relationship.
        [HttpDelete("mobo_remove/{pc_id}/{mobo_id}")]
        public async Task<ActionResult> RemoveMOBO(int pc_id, int mobo_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(mobo_id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            var relationship = await uow.PCMOBORepository.GetRelationshipAsync(mobo_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the MOBO!"));
            uow.PCMOBORepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the MOBO!"));
        }
        // Assign PC-RAM relationship.
        [HttpPut("ram_assign/{pc_id}/{ram_id}")]
        public async Task<ActionResult<RAMDTO>> AssignRAM(int pc_id, int ram_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var ram = await uow.RAMRepository.GetRAMByIDAsync(ram_id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            var relationship = await uow.PCRAMRepository.GetRelationshipAsync(ram_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_ram = new PC_RAM { PCID = pc_id, RAMID = ram_id };
            await uow.PCRAMRepository.AddPCRAMAsync(pc_ram);
            if (await uow.Complete()) return Ok(mapper.Map<RAMDTO>(ram));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the RAM!"));
        }
        // Get PC-RAM relationships.
        [HttpGet("ram_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<RAMDTO>>> GetRelatedRAMs(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCRAMRepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have RAMs assigned to!"));
            var rams = await uow.PCRAMRepository.GetRAMsDTOAsync(pc_id);
            if (rams == null) return NotFound(new ApiException(404, "Not Found", "Related RAMs not found!"));
            return Ok(mapper.Map<RAMDTO>(rams));
        }
        // Cancel PC-RAM relationship.
        [HttpDelete("ram_remove/{pc_id}/{ram_id}")]
        public async Task<ActionResult> RemoveRAM(int pc_id, int ram_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var ram = await uow.RAMRepository.GetRAMByIDAsync(ram_id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            var relationship = await uow.PCRAMRepository.GetRelationshipAsync(ram_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the RAM!"));
            uow.PCRAMRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the RAM!"));
        }
        // Assign PC-GPU relationship.
        [HttpPut("gpu_assign/{pc_id}/{gpu_id}")]
        public async Task<ActionResult<GPUDTO>> AssignGPU(int pc_id, int gpu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var gpu = await uow.GPURepository.GetGPUByIDAsync(gpu_id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            var relationship = await uow.PCGPURepository.GetRelationshipAsync(gpu_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_gpu = new PC_GPU { PCID = pc_id, GPUID = gpu_id };
            await uow.PCGPURepository.AddPCGPUAsync(pc_gpu);
            if (await uow.Complete()) return Ok(mapper.Map<GPUDTO>(gpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the GPU!"));
        }
        // Get PC-GPU relationships.
        [HttpGet("gpu_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<GPUDTO>>> GetRelatedGPUs(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCGPURepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have GPUs assigned to!"));
            var gpus = await uow.PCGPURepository.GetGPUsDTOAsync(pc_id);
            if (gpus == null) return NotFound(new ApiException(404, "Not Found", "Related GPUs not found!"));
            return Ok(mapper.Map<GPUDTO>(gpus));
        }
        // Cancel PC-GPU relationship.
        [HttpDelete("gpu_remove/{pc_id}/{gpu_id}")]
        public async Task<ActionResult> RemoveGPU(int pc_id, int gpu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var gpu = await uow.GPURepository.GetGPUByIDAsync(gpu_id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            var relationship = await uow.PCGPURepository.GetRelationshipAsync(gpu_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the GPU!"));
            uow.PCGPURepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the GPU!"));
        }
        // Assign PC-PSU relationship.
        [HttpPut("psu_assign/{pc_id}/{psu_id}")]
        public async Task<ActionResult<PSUDTO>> AssignPSU(int pc_id, int psu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var psu = await uow.PSURepository.GetPSUByIDAsync(psu_id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            var relationship = await uow.PCPSURepository.GetRelationshipAsync(psu_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_psu = new PC_PSU { PCID = pc_id, PSUID = psu_id };
            await uow.PCPSURepository.AddPCPSUAsync(pc_psu);
            if (await uow.Complete()) return Ok(mapper.Map<PSUDTO>(psu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the PSU!"));
        }
        // Get PC-PSU relationships.
        [HttpGet("psu_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<PSUDTO>>> GetRelatedPSUs(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCPSURepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have PSUs assigned to!"));
            var psus = await uow.PCPSURepository.GetPSUsDTOAsync(pc_id);
            if (psus == null) return NotFound(new ApiException(404, "Not Found", "Related PSUs not found!"));
            return Ok(mapper.Map<PSUDTO>(psus));
        }
        // Cancel PC-PSU relationship.
        [HttpDelete("psu_remove/{pc_id}/{psu_id}")]
        public async Task<ActionResult> RemovePSU(int pc_id, int psu_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var psu = await uow.PSURepository.GetPSUByIDAsync(psu_id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            var relationship = await uow.PCPSURepository.GetRelationshipAsync(psu_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the PSU!"));
            uow.PCPSURepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the PSU!"));
        }
        // Assign PC-Storage relationship.
        [HttpPut("storage_assign/{pc_id}/{storage_id}")]
        public async Task<ActionResult<StorageDTO>> AssignStorage(int pc_id, int storage_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var storage = await uow.StorageRepository.GetStorageByIDAsync(storage_id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            var relationship = await uow.PCStorageRepository.GetRelationshipAsync(storage_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_storage = new PC_Storage { PCID = pc_id, StorageID = storage_id };
            await uow.PCStorageRepository.AddPCStorageAsync(pc_storage);
            if (await uow.Complete()) return Ok(mapper.Map<StorageDTO>(storage));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the Storage!"));
        }
        // Get PC-Storage relationships.
        [HttpGet("storage_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<StorageDTO>>> GetRelatedStorages(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCStorageRepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have Storages assigned to!"));
            var storages = await uow.PCStorageRepository.GetStoragesDTOAsync(pc_id);
            if (storages == null) return NotFound(new ApiException(404, "Not Found", "Related Storages not found!"));
            return Ok(mapper.Map<StorageDTO>(storages));
        }
        // Cancel PC-Storage relationship.
        [HttpDelete("storage_remove/{pc_id}/{storage_id}")]
        public async Task<ActionResult> RemoveStorage(int pc_id, int storage_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var storage = await uow.StorageRepository.GetStorageByIDAsync(storage_id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            var relationship = await uow.PCStorageRepository.GetRelationshipAsync(storage_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the Storage!"));
            uow.PCStorageRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the Storage!"));
        }
        // Assign PC-NetworkCard relationship.
        [HttpPut("networkcard_assign/{pc_id}/{networkcard_id}")]
        public async Task<ActionResult<NetworkCardDTO>> AssignNetworkCard(int pc_id, int networkcard_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(networkcard_id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            var relationship = await uow.PCNetworkCardRepository.GetRelationshipAsync(networkcard_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_networkcard = new PC_NetworkCard { PCID = pc_id, NetworkCardID = networkcard_id };
            await uow.PCNetworkCardRepository.AddPCNetworkCardAsync(pc_networkcard);
            if (await uow.Complete()) return Ok(mapper.Map<NetworkCardDTO>(networkcard));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the NetworkCard!"));
        }
        // Get PC-NetworkCard relationships.
        [HttpGet("networkcard_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<NetworkCardDTO>>> GetRelatedNetworkCards(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCNetworkCardRepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have NetworkCards assigned to!"));
            var networkcards = await uow.PCNetworkCardRepository.GetNetworkCardsDTOAsync(pc_id);
            if (networkcards == null) return NotFound(new ApiException(404, "Not Found", "Related NetworkCards not found!"));
            return Ok(mapper.Map<NetworkCardDTO>(networkcards));
        }
        // Cancel PC-NetworkCard relationship.
        [HttpDelete("networkcard_remove/{pc_id}/{networkcard_id}")]
        public async Task<ActionResult> RemoveNetworkCard(int pc_id, int networkcard_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(networkcard_id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            var relationship = await uow.PCNetworkCardRepository.GetRelationshipAsync(networkcard_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the NetworkCard!"));
            uow.PCNetworkCardRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the NetworkCard!"));
        }
        // Assign PC-Monitor relationship.
        [HttpPut("monitor_assign/{pc_id}/{monitor_id}")]
        public async Task<ActionResult<MonitorDTO>> AssignMonitor(int pc_id, int monitor_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(monitor_id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            var relationship = await uow.PCMonitorRepository.GetRelationshipAsync(monitor_id, pc_id);
            if (relationship != null) return BadRequest(new ApiException(400, "Bad Request", "Relationship already exists!"));
            var pc_monitor = new PC_Monitor { PCID = pc_id, MonitorID = monitor_id };
            await uow.PCMonitorRepository.AddPCMonitorAsync(pc_monitor);
            if (await uow.Complete()) return Ok(mapper.Map<MonitorDTO>(monitor));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the Monitor!"));
        }
        // Get PC-Monitor relationships.
        [HttpGet("monitor_get/{pc_id}")]
        public async Task<ActionResult<IEnumerable<MonitorDTO>>> GetRelatedMonitors(int pc_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var relationships = await uow.PCMonitorRepository.GetRelationshipPCsAsync(pc_id);
            if (relationships == null) return NotFound(new ApiException(404, "Not Found", "PC does not have Monitors assigned to!"));
            var cpus = await uow.PCMonitorRepository.GetMonitorsDTOAsync(pc_id);
            if (cpus == null) return NotFound(new ApiException(404, "Not Found", "Related Monitors not found!"));
            return Ok(mapper.Map<MonitorDTO>(cpus));
        }
        // Cancel PC-Monitor relationship.
        [HttpDelete("monitor_remove/{pc_id}/{monitor_id}")]
        public async Task<ActionResult> RemoveMonitor(int pc_id, int monitor_id) {
            var pc = await uow.PCRepository.GetPCByIDAsync(pc_id);
            if (pc == null) return NotFound(new ApiException(404, "Not Found", "PC not found!"));
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(monitor_id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            var relationship = await uow.PCMonitorRepository.GetRelationshipAsync(monitor_id, pc_id);
            if (relationship == null) return BadRequest(new ApiException(400, "Bad Request", "There is not an existing relation between the PC and the Monitor!"));
            uow.PCMonitorRepository.Delete(relationship);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the relationship of the Monitor!"));
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.PCRepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.PCRepository.GetFilterModel();
        }
        [HttpGet("filter/domain")]
        public async Task<ActionResult<List<string>>> FilterDomain() {
            return await uow.PCRepository.GetFilterDomain();
        }
    }
}