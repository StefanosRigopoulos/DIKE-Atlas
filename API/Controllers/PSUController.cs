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
    public class PSUController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new PSU.
        [HttpPost("add")]
        public async Task<ActionResult<PSUDTO>> AddPSU(PSUDTO psuDTO) {
            // Check if PSU exists.
            if (psuDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete PSU data!"));
            // Assign the DTO to an entity.
            var psu = mapper.Map<PSU>(psuDTO);
            // Check if the entity is unique.
            if (psuDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.PSURepository.CheckUniquenessAsync(psu))
                    return BadRequest(new ApiException(400, "Bad Request", "PSU is already registered!"));
            psu.Barcode = BarcodeGenerator.GenerateBarcode("PSU");
            // Add the newly created PSU to the database.
            await uow.PSURepository.AddPSUAsync(psu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<PSUDTO>(psu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the PSU!"));
        }
        // Update an existing PSU
        [HttpPut("update/{id}")]
        public async Task<ActionResult<PSUDTO>> UpdatePSU(int id, PSUDTO psuDTO) {
            // Check if PSU exists.
            var psu = await uow.PSURepository.GetPSUByIDAsync(id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            // Map the DTO variables to the PSU entity.
            mapper.Map(psuDTO, psu);
            // Update the database entity with the new data.
            uow.PSURepository.Update(psu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<PSUDTO>(psu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the PSU!"));
        }
        // Delete an existing PSU
        [HttpDelete("delete/{psu_id}")]
        public async Task<ActionResult> DeletePSU(int psu_id) {
            // Check if PSU exists.
            var psu = await uow.PSURepository.GetPSUByIDAsync(psu_id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            // Delete the relationships the PSU has if it exists.
            var relationships = await uow.PCPSURepository.GetRelationshipPSUsAsync(psu_id);
            if (relationships != null) {
                foreach (PC_PSU relationship in relationships) {
                    uow.PCPSURepository.Delete(relationship);
                }
            }
            // Delete the PSU itself.
            uow.PSURepository.Delete(psu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the PSU!"));
        }
        // Get specific PSU based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<PSUDTO?>> GetPSU(int id)
        {
            var psu = await uow.PSURepository.GetPSUByIDAsync(id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            return Ok(mapper.Map<PSUDTO>(psu));
        }
        // Get PSUs with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<PSUDTO>>> GetPagedPSUs([FromQuery]PSUParams psuParams)
        {
            var pagedPSUs = await uow.PSURepository.GetPSUsWithParametersAsync(psuParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedPSUs.CurrentPage, pagedPSUs.PageSize, pagedPSUs.TotalCount, pagedPSUs.TotalPages));
            return Ok(pagedPSUs);
        }
        // Get all PSUs.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<PSUDTO?>>> GetPSUs()
        {
            var psus = await uow.PSURepository.GetPSUsDTOAsync();
            return Ok(psus);
        }
        // Get Related PC with the PSU given.
        [HttpGet("get/related_pc/{psu_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int psu_id)
        {
            var psu = await uow.PSURepository.GetPSUByIDAsync(psu_id);
            if (psu == null) return NotFound(new ApiException(404, "Not Found", "PSU not found!"));
            var relationship = await uow.PCPSURepository.GetRelationshipPSUsAsync(psu_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "PSU does not have PCs assigned to!"));
            var pcs = await uow.PCPSURepository.GetPCsDTOAsync(psu_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.PSURepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.PSURepository.GetFilterModel();
        }
    }
}