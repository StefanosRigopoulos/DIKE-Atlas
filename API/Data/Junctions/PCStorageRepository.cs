using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCStorageRepository(DataContext context, IMapper mapper) : IPCStorageRepository
    {
        public void Delete(PC_Storage pc_storage)
        {
            context.Remove(pc_storage);
        }
        public async Task AddPCStorageAsync(PC_Storage pc_storage)
        {
            await context.PC_Storages.AddAsync(pc_storage);
        }
        public async Task<PC_Storage?> GetRelationshipAsync(int storageid, int pcid)
        {
            return await context.PC_Storages
                                .FirstOrDefaultAsync(x => x.StorageID == storageid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_Storage>> GetRelationshipStoragesAsync(int storageid)
        {
            return await context.PC_Storages
                                .Where(x => x.StorageID == storageid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_Storage>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_Storages
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_Storage>> GetRelationshipsAsync()
        {
            return await context.PC_Storages
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int storageid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_Storages.Any(pc_storage => pc_storage.StorageID == storageid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<Storage>> GetStoragesAsync(int pcid)
        {
            return await context.Storages
                                .Where(storage => storage.PC_Storages.Any(pc_storage => pc_storage.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int storageid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_Storages.Any(pc_storage => pc_storage.StorageID == storageid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<StorageDTO>> GetStoragesDTOAsync(int pcid)
        {
            return await context.Storages
                                .Where(storage => storage.PC_Storages.Any(pc_storage => pc_storage.PCID == pcid))
                                .ProjectTo<StorageDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}