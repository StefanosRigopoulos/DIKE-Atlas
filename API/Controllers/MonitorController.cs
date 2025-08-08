using API.DTOs;
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
    public class MonitorController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new Monitor.
        [HttpPost("add")]
        public async Task<ActionResult<MonitorDTO>> AddMonitor(MonitorDTO monitorDTO) {
            // Check if Monitor exists.
            if (monitorDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete Monitor data!"));
            // Assign the DTO to an entity.
            var monitor = mapper.Map<Entities.Monitor>(monitorDTO);
            // Check if the entity is unique.
            if (monitorDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.MonitorRepository.CheckUniquenessAsync(monitor))
                    return BadRequest(new ApiException(400, "Bad Request", "Monitor is already registered!"));
            monitor.Barcode = BarcodeGenerator.GenerateBarcode("MONI");
            // Add the newly created Monitor to the database.
            await uow.MonitorRepository.AddMonitorAsync(monitor);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<MonitorDTO>(monitor));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the Monitor!"));
        }
        // Update an existing Monitor
        [HttpPut("update/{id}")]
        public async Task<ActionResult<MonitorDTO>> UpdateMonitor(int id, MonitorDTO monitorDTO) {
            // Check if Monitor exists.
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            // Map the DTO variables to the Monitor entity.
            mapper.Map(monitorDTO, monitor);
            // Update the database entity with the new data.
            uow.MonitorRepository.Update(monitor);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<MonitorDTO>(monitor));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the Monitor!"));
        }
        // Delete an existing Monitor
        [HttpDelete("delete/{monitor_id}")]
        public async Task<ActionResult> DeleteMonitor(int monitor_id) {
            // Check if Monitor exists.
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(monitor_id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            // Delete the relationships the Monitor has if it exists.
            var relationships = await uow.PCMonitorRepository.GetRelationshipMonitorsAsync(monitor_id);
            if (relationships != null) {
                foreach (PC_Monitor relationship in relationships) {
                    uow.PCMonitorRepository.Delete(relationship);
                }
            }
            // Delete the Monitor itself.
            uow.MonitorRepository.Delete(monitor);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the Monitor!"));
        }
        // Get specific Monitor based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<MonitorDTO?>> GetMonitor(int id)
        {
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            return Ok(mapper.Map<MonitorDTO>(monitor));
        }
        // Get Monitors with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<MonitorDTO>>> GetPagedMonitors([FromQuery]MonitorParams monitorParams)
        {
            var pagedMonitors = await uow.MonitorRepository.GetMonitorsWithParametersAsync(monitorParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedMonitors.CurrentPage, pagedMonitors.PageSize, pagedMonitors.TotalCount, pagedMonitors.TotalPages));
            return Ok(pagedMonitors);
        }
        // Get all Monitors.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<MonitorDTO?>>> GetMonitors()
        {
            var monitors = await uow.MonitorRepository.GetMonitorsDTOAsync();
            return Ok(monitors);
        }
        // Get Related PC with the Monitor given.
        [HttpGet("get/related_pc/{monitor_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int monitor_id)
        {
            var monitor = await uow.MonitorRepository.GetMonitorByIDAsync(monitor_id);
            if (monitor == null) return NotFound(new ApiException(404, "Not Found", "Monitor not found!"));
            var relationship = await uow.PCMonitorRepository.GetRelationshipMonitorsAsync(monitor_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "Monitor does not have PCs assigned to!"));
            var pcs = await uow.PCMonitorRepository.GetPCsDTOAsync(monitor_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.MonitorRepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.MonitorRepository.GetFilterModel();
        }
    }
}