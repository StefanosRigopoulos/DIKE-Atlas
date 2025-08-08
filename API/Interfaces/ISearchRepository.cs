using API.DTOs;

namespace API.Interfaces
{
    public interface ISearchRepository
    {
        Task<IEnumerable<SearchResultDTO>> SearchAsync(string searchTerm);
    }
}