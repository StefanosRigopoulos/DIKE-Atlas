using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface INetworkCardRepository
    {
        void Update(NetworkCard networkCard);
        void Delete(NetworkCard networkCard);
        Task<bool> CheckUniquenessAsync(NetworkCard networkcard);
        Task AddNetworkCardAsync(NetworkCard networkcard);
        Task<PagedList<NetworkCardDTO>> GetNetworkCardsWithParametersAsync(NetworkCardParams networkcardParams);
        Task<NetworkCard?> GetNetworkCardByIDAsync(int id);
        Task<NetworkCardDTO?> GetNetworkCardDTOByIDAsync(int id);
        Task<IEnumerable<NetworkCard>> GetNetworkCardsAsync();
        Task<IEnumerable<NetworkCardDTO>> GetNetworkCardsDTOAsync();
        Task<List<string>> GetFilterBrand();
        Task<List<string>> GetFilterModel();
    }
}