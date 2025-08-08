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
    public class GPUController(IUnitOfWork uow, IMapper mapper) : BaseAPIController
    {
        // Add a new GPU.
        [HttpPost("add")]
        public async Task<ActionResult<GPUDTO>> AddGPU(GPUDTO gpuDTO) {
            // Check if GPU exists.
            if (gpuDTO == null) return BadRequest(new ApiException(400, "Bad Request", "Invalid or Incomplete GPU data!"));
            // Assign the DTO to an entity.
            var gpu = mapper.Map<GPU>(gpuDTO);
            // Check if the entity is unique.
            if (gpuDTO.SerialNumber != "")  // Make sure its not empty.
                if (await uow.GPURepository.CheckUniquenessAsync(gpu))
                    return BadRequest(new ApiException(400, "Bad Request", "GPU is already registered!"));
            gpu.Barcode = BarcodeGenerator.GenerateBarcode("GPU");
            // Add the newly created GPU to the database.
            await uow.GPURepository.AddGPUAsync(gpu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<GPUDTO>(gpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to add the GPU!"));
        }
        // Update an existing GPU
        [HttpPut("update/{id}")]
        public async Task<ActionResult<GPUDTO>> UpdateGPU(int id, GPUDTO gpuDTO) {
            // Check if GPU exists.
            var gpu = await uow.GPURepository.GetGPUByIDAsync(id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            // Map the DTO variables to the GPU entity.
            mapper.Map(gpuDTO, gpu);
            // Update the database entity with the new data.
            uow.GPURepository.Update(gpu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok(mapper.Map<GPUDTO>(gpu));
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to update the GPU!"));
        }
        // Delete an existing GPU
        [HttpDelete("delete/{gpu_id}")]
        public async Task<ActionResult> DeleteGPU(int gpu_id) {
            // Check if GPU exists.
            var gpu = await uow.GPURepository.GetGPUByIDAsync(gpu_id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            // Delete the relationships the GPU has if it exists.
            var relationships = await uow.PCGPURepository.GetRelationshipGPUsAsync(gpu_id);
            if (relationships != null) {
                foreach (PC_GPU relationship in relationships) {
                    uow.PCGPURepository.Delete(relationship);
                }
            }
            // Delete the GPU itself.
            uow.GPURepository.Delete(gpu);
            // Save and complete the process.
            if (await uow.Complete()) return Ok();
            return BadRequest(new ApiException(400, "Bad Request", "Error while trying to delete the GPU!"));
        }
        // Get specific GPU based on ID.
        [HttpGet("get/{id}")]
        public async Task<ActionResult<GPUDTO?>> GetGPU(int id)
        {
            var gpu = await uow.GPURepository.GetGPUByIDAsync(id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            return Ok(mapper.Map<GPUDTO>(gpu));
        }
        // Get GPUs with pagination.
        [HttpGet("get/paged")]
        public async Task<ActionResult<PagedList<GPUDTO>>> GetPagedGPUs([FromQuery]GPUParams gpuParams)
        {
            var pagedGPUs = await uow.GPURepository.GetGPUsWithParametersAsync(gpuParams);
            Response.AddPaginationHeader(new PaginationHeader(pagedGPUs.CurrentPage, pagedGPUs.PageSize, pagedGPUs.TotalCount, pagedGPUs.TotalPages));
            return Ok(pagedGPUs);
        }
        // Get all GPUs.
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<GPUDTO?>>> GetGPUs()
        {
            var gpus = await uow.GPURepository.GetGPUsDTOAsync();
            return Ok(gpus);
        }
        // Get Related PC with the GPU given.
        [HttpGet("get/related_pc/{gpu_id}")]
        public async Task<ActionResult<IEnumerable<PCDTO>>> GetRelatedPC(int gpu_id)
        {
            var gpu = await uow.GPURepository.GetGPUByIDAsync(gpu_id);
            if (gpu == null) return NotFound(new ApiException(404, "Not Found", "GPU not found!"));
            var relationship = await uow.PCGPURepository.GetRelationshipGPUsAsync(gpu_id);
            if (relationship == null) return NotFound(new ApiException(404, "Not Found", "GPU does not have PCs assigned to!"));
            var pcs = await uow.PCGPURepository.GetPCsDTOAsync(gpu_id);
            if (pcs == null) return NotFound(new ApiException(404, "Not Found", "Related PCs not found!"));
            return Ok(pcs);
        }

        // Filter Options
        [HttpGet("filter/brand")]
        public async Task<ActionResult<List<string>>> FilterBrand() {
            return await uow.GPURepository.GetFilterBrand();
        }
        [HttpGet("filter/model")]
        public async Task<ActionResult<List<string>>> FilterModel() {
            return await uow.GPURepository.GetFilterModel();
        }
    }
}