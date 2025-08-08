using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class NetworkCardRepository(DataContext context, IMapper mapper) : INetworkCardRepository
    {
        public void Update(NetworkCard networkcard)
        {
            context.Entry(networkcard).State = EntityState.Modified;
        }
        public void Delete(NetworkCard networkcard)
        {
            context.Remove(networkcard);
        }
        public async Task<bool> CheckUniquenessAsync(NetworkCard networkcard) {
            return await context.NetworkCards.AnyAsync(x => x.SerialNumber == networkcard.SerialNumber);
        }
        public async Task AddNetworkCardAsync(NetworkCard networkcard)
        {
            await context.NetworkCards.AddAsync(networkcard);
        }
        public async Task<PagedList<NetworkCardDTO>> GetNetworkCardsWithParametersAsync(NetworkCardParams networkcardParams)
        {
            var query = context.NetworkCards.AsQueryable();

            // Apply filtering based on NetworkCardParams.
            if (!string.IsNullOrEmpty(networkcardParams.Brand))
                query = query.Where(networkcard => networkcard.Brand.Contains(networkcardParams.Brand));
            if (!string.IsNullOrEmpty(networkcardParams.Model))
                query = query.Where(networkcard => networkcard.Model.Contains(networkcardParams.Model));

            // Apply sorting.
            query = networkcardParams.OrderBy switch
            {
                "brand" => query.OrderBy(u => u.Brand),
                "model" => query.OrderBy(u => u.Model),
                _ => query.OrderBy(u => u.Brand)
            };

            // Project to DTO and paginate
            var pagedList = await PagedList<NetworkCardDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<NetworkCardDTO>(mapper.ConfigurationProvider),
                networkcardParams.PageNumber,
                networkcardParams.PageSize);
            return pagedList;
        }
        public async Task<NetworkCard?> GetNetworkCardByIDAsync(int id)
        {
            return await context.NetworkCards
                                .Include(x => x.PC_NetworkCards)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<NetworkCardDTO?> GetNetworkCardDTOByIDAsync(int id)
        {
            return await context.NetworkCards
                                .Include(x => x.PC_NetworkCards)
                                .ProjectTo<NetworkCardDTO>(mapper.ConfigurationProvider)
                                .FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<IEnumerable<NetworkCard>> GetNetworkCardsAsync()
        {
            return await context.NetworkCards
                                .Include(x => x.PC_NetworkCards)
                                .ToListAsync();
        }
        public async Task<IEnumerable<NetworkCardDTO>> GetNetworkCardsDTOAsync()
        {
            return await context.NetworkCards
                                .Include(x => x.PC_NetworkCards)
                                .ProjectTo<NetworkCardDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterBrand() {
            return await context.NetworkCards
                                .Where(u => u.Brand != null && u.Brand != "")
                                .Select(u => u.Brand)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
        public async Task<List<string>> GetFilterModel() {
            return await context.NetworkCards
                                .Where(u => u.Model != null && u.Model != "")
                                .Select(u => u.Model)
                                .Distinct()
                                .OrderBy(u => u)
                                .ToListAsync();
        }
    }
}