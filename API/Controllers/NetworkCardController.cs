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
    public class NetworkCardController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new NetworkCard.
        [HttpPost("add")]
        public async Task<ActionResult<NetworkCardDTO>> AddNetworkCard(NetworkCardDTO networkcardDTO) {
            // Check if NetworkCard exists.
            if (networkcardDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete NetworkCard data!"));
            // Assign the DTO to an entity.
            var networkcard = mapper.Map<NetworkCard>(networkcardDTO);
            // Check if the entity is unique.
            if (networkcardDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.NetworkCardRepository.CheckUniquenessAsync(networkcard))
                    return BadRequest(new ApiException(400, "Bad Request", "NetworkCard is already registered!"));
            networkcard.Barcode = BarcodeGenerator.GenerateBarcode("NET");
            // Add the newly created NetworkCard to the database.
            await uow.NetworkCardRepository.AddNetworkCardAsync(networkcard);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<NetworkCardDTO>(networkcard));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the NetworkCard!"));
        }
        // Update an existing NetworkCard
        [HttpPut("update/{id}")]
        public async Task<ActionResult<NetworkCardDTO>> UpdateNetworkCard(int id, NetworkCardDTO networkcardDTO) {
            // Check if NetworkCard exists.
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            // Map the DTO variables to the NetworkCard entity.
            mapper.Map(networkcardDTO, networkcard);
            // Update the database entity with the new data.
            uow.NetworkCardRepository.Update(networkcard);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<NetworkCardDTO>(networkcard));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the NetworkCard!"));
        }
        // Delete an existing NetworkCard
        [HttpDelete("delete/{networkcard_id}")]
        public async Task<ActionResult> DeleteNetworkCard(int networkcard_id) {
            // Check if NetworkCard exists.
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(networkcard_id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            // Delete the relationships the NetworkCard has if it exists.
            var relationships = await uow.PCNetworkCardRepository.GetRelationshipNetworkCardsAsync(networkcard_id);
            if (relationships != null) {
                foreach (PC_NetworkCard relationship in relationships) {
                    uow.PCNetworkCardRepository.Delete(relationship);
                }
            }
            // Delete the NetworkCard itself.
            uow.NetworkCardRepository.Delete(networkcard);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the NetworkCard!"));
        }
        // Get specific NetworkCard based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<NetworkCardDTO?>> GetNetworkCard(int id)
        {
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            return Ok(mapper.Map<NetworkCardDTO>(networkcard));
        }
        // Get NetworkCards with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<NetworkCardDTO>>> GetPagedNetworkCards([FromQuery]NetworkCardParams networkcardParams)
        {
            var pagedNetworkCards = await uow.NetworkCardRepository.GetNetworkCardsWithParametersAsync(networkcardParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedNetworkCards.CurrentPage, pagedNetworkCards.PageSize, pagedNetworkCards.TotalCount, pagedNetworkCards.TotalPages));
            return Ok(pagedNetworkCards);
        }
        // Get all NetworkCards.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<NetworkCardDTO?>>> GetNetworkCards()
        {
            var networkcards = await uow.NetworkCardRepository.GetNetworkCardsDTOAsync();
            return Ok(networkcards);
        }
        // Get Related PC with the NetworkCard given.
        [HttpGet("get/related_pc/{networkcard_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int networkcard_id)
        {
            var networkcard = await uow.NetworkCardRepository.GetNetworkCardByIDAsync(networkcard_id);
            if (networkcard == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard not found!"));
            var relationship = await uow.PCNetworkCardRepository.GetRelationshipNetworkCardsAsync(networkcard_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "NetworkCard does not have PCs assigned to!"));
            var pcs = await uow.PCNetworkCardRepository.GetPCsDTOAsync(networkcard_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }

        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.NetworkCardRepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.NetworkCardRepository.GetFilterModel();
        }
    }
}