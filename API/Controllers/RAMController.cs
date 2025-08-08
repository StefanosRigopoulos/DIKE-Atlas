using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
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
    public class RAMController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new RAM.
        [HttpPost("add")]
        public async Task<ActionResult<RAMDTO>> AddRAM(RAMDTO ramDTO) {
            // Check if RAM exists.
            if (ramDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete RAM data!"));
            // Assign the DTO to an entity.
            var ram = mapper.Map<RAM>(ramDTO);
            // Check if the entity is unique.
            if (ramDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.RAMRepository.CheckUniquenessAsync(ram))
                    return BadRequest(new ApiException(400, "Bad Request", "RAM is already registered!"));
            ram.Barcode = BarcodeGenerator.GenerateBarcode("RAM");
            // Add the newly created RAM to the database.
            await uow.RAMRepository.AddRAMAsync(ram);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<RAMDTO>(ram));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the RAM!"));
        }
        // Update an existing RAM
        [HttpPut("update/{id}")]
        public async Task<ActionResult<RAMDTO>> UpdateRAM(int id, RAMDTO ramDTO) {
            // Check if RAM exists.
            var ram = await uow.RAMRepository.GetRAMByIDAsync(id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            // Map the DTO variables to the RAM entity.
            mapper.Map(ramDTO, ram);
            // Update the database entity with the new data.
            uow.RAMRepository.Update(ram);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<RAMDTO>(ram));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the RAM!"));
        }
        // Delete an existing RAM
        [HttpDelete("delete/{ram_id}")]
        public async Task<ActionResult> DeleteRAM(int ram_id) {
            // Check if RAM exists.
            var ram = await uow.RAMRepository.GetRAMByIDAsync(ram_id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            // Delete the relationships the RAM has if it exists.
            var relationships = await uow.PCRAMRepository.GetRelationshipRAMsAsync(ram_id);
            if (relationships != null) {
                foreach (PC_RAM relationship in relationships) {
                    uow.PCRAMRepository.Delete(relationship);
                }
            }
            // Delete the RAM itself.
            uow.RAMRepository.Delete(ram);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the RAM!"));
        }
        // Get specific RAM based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<RAMDTO?>> GetRAM(int id)
        {
            var ram = await uow.RAMRepository.GetRAMByIDAsync(id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            return Ok(mapper.Map<RAMDTO>(ram));
        }
        // Get RAMs with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<RAMDTO>>> GetPagedRAMs([FromQuery]RAMParams ramParams)
        {
            var pagedRAMs = await uow.RAMRepository.GetRAMsWithParametersAsync(ramParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedRAMs.CurrentPage, pagedRAMs.PageSize, pagedRAMs.TotalCount, pagedRAMs.TotalPages));
            return Ok(pagedRAMs);
        }
        // Get all RAMs.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<RAMDTO?>>> GetRAMs()
        {
            var rams = await uow.RAMRepository.GetRAMsDTOAsync();
            return Ok(rams);
        }
        // Get Related PC with the RAM given.
        [HttpGet("get/related_pc/{ram_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int ram_id)
        {
            var ram = await uow.RAMRepository.GetRAMByIDAsync(ram_id);
            if (ram == null) return NotFound(new ApiException(404, "Not Found", "RAM not found!"));
            var relationship = await uow.PCRAMRepository.GetRelationshipRAMsAsync(ram_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "RAM does not have PCs assigned to!"));
            var pcs = await uow.PCRAMRepository.GetPCsDTOAsync(ram_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.RAMRepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.RAMRepository.GetFilterModel();
        }
    }
}