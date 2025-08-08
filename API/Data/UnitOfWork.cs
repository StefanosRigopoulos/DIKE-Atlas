using API.Data.Junctions;
using API.Interfaces;
using API.Interfaces.Junctions;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork(DataContext context, IMapper mapper) : IUnitOfWork
    {
        public IUserRepository UserRepository => new UserRepository(context, mapper);
        public IFileRepository FileRepository => new FileRepository(context);
        public ISearchRepository SearchRepository => new SearchRepository(context);
        public IEmployeeRepository EmployeeRepository => new EmployeeRepository(context, mapper);
        public IPCRepository PCRepository => new PCRepository(context, mapper);
        public ICPURepository CPURepository => new CPURepository(context, mapper);
        public IMOBORepository MOBORepository => new MOBORepository(context, mapper);
        public IRAMRepository RAMRepository => new RAMRepository(context, mapper);
        public IGPURepository GPURepository => new GPURepository(context, mapper);
        public IPSURepository PSURepository => new PSURepository(context, mapper);
        public IStorageRepository StorageRepository => new StorageRepository(context, mapper);
        public INetworkCardRepository NetworkCardRepository => new NetworkCardRepository(context, mapper);
        public IMonitorRepository MonitorRepository => new MonitorRepository(context, mapper);
        
        // Junction
        public IEmployeePCRepository EmployeePCRepository => new EmployeePCRepository(context, mapper);
        public IPCCPURepository PCCPURepository => new PCCPURepository(context, mapper);
        public IPCMOBORepository PCMOBORepository => new PCMOBORepository(context, mapper);
        public IPCRAMRepository PCRAMRepository => new PCRAMRepository(context, mapper);
        public IPCGPURepository PCGPURepository => new PCGPURepository(context, mapper);
        public IPCPSURepository PCPSURepository => new PCPSURepository(context, mapper);
        public IPCStorageRepository PCStorageRepository => new PCStorageRepository(context, mapper);
        public IPCNetworkCardRepository PCNetworkCardRepository => new PCNetworkCardRepository(context, mapper);
        public IPCMonitorRepository PCMonitorRepository => new PCMonitorRepository(context, mapper);

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync() > 0;
        }
        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}