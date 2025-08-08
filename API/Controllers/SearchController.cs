using API.Interfaces;
using API.Errors;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class SearchController(IUnitOfWork uow) : BaseAPIController
    {
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new ApiException(400, "Bad Request", "Search term is required."));
            var results = await uow.SearchRepository.SearchAsync(searchTerm);
            return Ok(results);
        }
    }
}