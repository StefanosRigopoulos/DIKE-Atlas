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
    public class MOBOController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new MOBO.
        [HttpPost("add")]
        public async Task<ActionResult<MOBODTO>> AddMOBO(MOBODTO moboDTO) {
            // Check if MOBO exists.
            if (moboDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete MOBO data!"));
            // Assign the DTO to an entity.
            var mobo = mapper.Map<MOBO>(moboDTO);
            // Check if the entity is unique.
            if (moboDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.MOBORepository.CheckUniquenessAsync(mobo))
                    return BadRequest(new ApiException(400, "Bad Request", "MOBO is already registered!"));
            mobo.Barcode = BarcodeGenerator.GenerateBarcode("MOBO");
            // Add the newly created MOBO to the database.
            await uow.MOBORepository.AddMOBOAsync(mobo);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<MOBODTO>(mobo));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the MOBO!"));
        }

        // Update an existing MOBO
        [HttpPut("update/{id}")]
        public async Task<ActionResult<MOBODTO>> UpdateMOBO(int id, MOBODTO moboDTO) {
            // Check if MOBO exists.
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            // Map the DTO variables to the MOBO entity.
            mapper.Map(moboDTO, mobo);
            // Update the database entity with the new data.
            uow.MOBORepository.Update(mobo);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<MOBODTO>(mobo));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the MOBO!"));
        }

        // Delete an existing MOBO
        [HttpDelete("delete/{mobo_id}")]
        public async Task<ActionResult> DeleteMOBO(int mobo_id) {
            // Check if MOBO exists.
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(mobo_id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            // Delete the relationships the MOBO has if it exists.
            var relationships = await uow.PCMOBORepository.GetRelationshipMOBOsAsync(mobo_id);
            if (relationships != null) {
                foreach (PC_MOBO relationship in relationships) {
                    uow.PCMOBORepository.Delete(relationship);
                }
            }
            // Delete the MOBO itself.
            uow.MOBORepository.Delete(mobo);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the MOBO!"));
        }

        // Get specific MOBO based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<MOBODTO?>> GetMOBO(int id)
        {
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            return Ok(mapper.Map<MOBODTO>(mobo));
        }
        // Get MOBOs with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<MOBODTO>>> GetPagedMOBOs([FromQuery]MOBOParams moboParams)
        {
            var pagedMOBOs = await uow.MOBORepository.GetMOBOsWithParametersAsync(moboParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedMOBOs.CurrentPage, pagedMOBOs.PageSize, pagedMOBOs.TotalCount, pagedMOBOs.TotalPages));
            return Ok(pagedMOBOs);
        }
        // Get all MOBOs.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<MOBODTO?>>> GetMOBOs()
        {
            var mobos = await uow.MOBORepository.GetMOBOsDTOAsync();
            return Ok(mobos);
        }
        // Get Related PC with the MOBO given.
        [HttpGet("get/related_pc/{mobo_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int mobo_id)
        {
            var mobo = await uow.MOBORepository.GetMOBOByIDAsync(mobo_id);
            if (mobo == null) return NotFound(new ApiException(404, "Not Found", "MOBO not found!"));
            var relationship = await uow.PCMOBORepository.GetRelationshipMOBOsAsync(mobo_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "MOBO does not have PCs assigned to!"));
            var pcs = await uow.PCMOBORepository.GetPCsDTOAsync(mobo_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.MOBORepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.MOBORepository.GetFilterModel();
        }
    }
}