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
    public class StorageController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new Storage.
        [HttpPost("add")]
        public async Task<ActionResult<StorageDTO>> AddStorage(StorageDTO storageDTO) {
            // Check if Storage exists.
            if (storageDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete Storage data!"));
            // Assign the DTO to an entity.
            var storage = mapper.Map<Storage>(storageDTO);
            // Check if the entity is unique.
            if (storageDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.StorageRepository.CheckUniquenessAsync(storage))
                    return BadRequest(new ApiException(400, "Bad Request", "Storage is already registered!"));
            storage.Barcode = BarcodeGenerator.GenerateBarcode("STOR");
            // Add the newly created Storage to the database.
            await uow.StorageRepository.AddStorageAsync(storage);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<StorageDTO>(storage));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the Storage!"));
        }
        // Update an existing Storage
        [HttpPut("update/{id}")]
        public async Task<ActionResult<StorageDTO>> UpdateStorage(int id, StorageDTO storageDTO) {
            // Check if Storage exists.
            var storage = await uow.StorageRepository.GetStorageByIDAsync(id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            // Map the DTO variables to the Storage entity.
            mapper.Map(storageDTO, storage);
            // Update the database entity with the new data.
            uow.StorageRepository.Update(storage);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<StorageDTO>(storage));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the Storage!"));
        }
        // Delete an existing Storage
        [HttpDelete("delete/{storage_id}")]
        public async Task<ActionResult> DeleteStorage(int storage_id) {
            // Check if Storage exists.
            var storage = await uow.StorageRepository.GetStorageByIDAsync(storage_id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            // Delete the relationships the Storage has if it exists.
            var relationships = await uow.PCStorageRepository.GetRelationshipStoragesAsync(storage_id);
            if (relationships != null) {
                foreach (PC_Storage relationship in relationships) {
                    uow.PCStorageRepository.Delete(relationship);
                }
            }
            // Delete the Storage itself.
            uow.StorageRepository.Delete(storage);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the Storage!"));
        }
        // Get specific Storage based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<StorageDTO?>> GetStorage(int id)
        {
            var storage = await uow.StorageRepository.GetStorageByIDAsync(id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            return Ok(mapper.Map<StorageDTO>(storage));
        }
        // Get Storages with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<StorageDTO>>> GetPagedStorages([FromQuery]StorageParams storageParams)
        {
            var pagedStorages = await uow.StorageRepository.GetStoragesWithParametersAsync(storageParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedStorages.CurrentPage, pagedStorages.PageSize, pagedStorages.TotalCount, pagedStorages.TotalPages));
            return Ok(pagedStorages);
        }
        // Get all Storages.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<StorageDTO?>>> GetStorages()
        {
            var storages = await uow.StorageRepository.GetStoragesDTOAsync();
            return Ok(storages);
        }
        // Get Related PC with the Storage given.
        [HttpGet("get/related_pc/{storage_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int storage_id)
        {
            var storage = await uow.StorageRepository.GetStorageByIDAsync(storage_id);
            if (storage == null) return NotFound(new ApiException(404, "Not Found", "Storage not found!"));
            var relationship = await uow.PCStorageRepository.GetRelationshipStoragesAsync(storage_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "Storage does not have PCs assigned to!"));
            var pcs = await uow.PCStorageRepository.GetPCsDTOAsync(storage_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }
        
        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.StorageRepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.StorageRepository.GetFilterModel();
        }
        [HttpGet("filter/type")]
        public async Task<ActionResult<List<string>>> FilterType() {
            return await uow.StorageRepository.GetFilterType();
        }
        [HttpGet("filter/interface")]
        public async Task<ActionResult<List<string>>> FilterInterface() {
            return await uow.StorageRepository.GetFilterInterface();
        }
    }
}