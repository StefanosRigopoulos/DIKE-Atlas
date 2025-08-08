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
    public class CPUController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new CPU.
        [HttpPost("add")]
        public async Task<ActionResult<CPUDTO>> AddCPU(CPUDTO cpuDTO) {
            if (cpuDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete CPU data!"));
            var cpu = mapper.Map<CPU>(cpuDTO);
            if (cpuDTO.SerialNumber != "")
                if (await uow.CPURepository.CheckUniquenessAsync(cpu))
                    return BadRequest(new ApiException(400, "Bad Request", "CPU is already registered!"));
            cpu.Barcode = BarcodeGenerator.GenerateBarcode("CPU");
            await uow.CPURepository.AddCPUAsync(cpu);
            if (await uow.Complete()) return Ok(mapper.Map<CPUDTO>(cpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the CPU!"));
        }
        // Update an existing CPU
        [HttpPut("update/{id}")]
        public async Task<ActionResult<CPUDTO>> UpdateCPU(int id, CPUDTO cpuDTO) {
            var cpu = await uow.CPURepository.GetCPUByIDAsync(id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            mapper.Map(cpuDTO, cpu);
            uow.CPURepository.Update(cpu);
            if (await uow.Complete()) return Ok(mapper.Map<CPUDTO>(cpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the CPU!"));
        }
        // Delete an existing CPU
        [HttpDelete("delete/{cpu_id}")]
        public async Task<ActionResult> DeleteCPU(int cpu_id) {
            var cpu = await uow.CPURepository.GetCPUByIDAsync(cpu_id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            var relationships = await uow.PCCPURepository.GetRelationshipCPUsAsync(cpu_id);
            if (relationships != null) {
                foreach (PC_CPU relationship in relationships) {
                    uow.PCCPURepository.Delete(relationship);
                }
            }
            uow.CPURepository.Delete(cpu);
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the CPU!"));
        }

        // Get specific CPU based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<CPUDTO?>> GetCPU(int id)
        {
            var cpu = await uow.CPURepository.GetCPUByIDAsync(id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            return Ok(mapper.Map<CPUDTO>(cpu));
        }
        // Get CPUs with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<CPUDTO>>> GetPagedCPUs([FromQuery]CPUParams cpuParams)
        {
            var pagedCPUs = await uow.CPURepository.GetCPUsWithParametersAsync(cpuParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedCPUs.CurrentPage, pagedCPUs.PageSize, pagedCPUs.TotalCount, pagedCPUs.TotalPages));
            return Ok(pagedCPUs);
        }
        // Get all CPUs.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<CPUDTO?>>> GetCPUs()
        {
            var cpus = await uow.CPURepository.GetCPUsDTOAsync();
            return Ok(cpus);
        }
        // Get Related PC with the CPU given.
        [HttpGet("get/related_pc/{cpu_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int cpu_id)
        {
            var cpu = await uow.CPURepository.GetCPUByIDAsync(cpu_id);
            if (cpu == null) return NotFound(new ApiException(404, "Not Found", "CPU not found!"));
            var relationship = await uow.PCCPURepository.GetRelationshipCPUsAsync(cpu_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "CPU does not have PCs assigned to!"));
            var pcs = await uow.PCCPURepository.GetPCsDTOAsync(cpu_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.CPURepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.CPURepository.GetFilterModel();
        }
        [HttpGet("filter/core")]
        public async Task<ActionResult<List<string>>> FilterCore() {
            return await uow.CPURepository.GetFilterCore();
        }
    }
}