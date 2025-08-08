using API.Interfaces.Junctions;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IFileRepository FileRepository { get; }
        ISearchRepository SearchRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IPCRepository PCRepository { get; }
        ICPURepository CPURepository { get; }
        IMOBORepository MOBORepository { get; }
        IRAMRepository RAMRepository { get; }
        IGPURepository GPURepository { get; }
        IPSURepository PSURepository { get; }
        INetworkCardRepository NetworkCardRepository { get; }
        IMonitorRepository MonitorRepository { get; }
        IStorageRepository StorageRepository { get; }

        // Junction Repositories
        IEmployeePCRepository EmployeePCRepository { get; }
        IPCCPURepository PCCPURepository { get; }
        IPCMOBORepository PCMOBORepository { get; }
        IPCRAMRepository PCRAMRepository { get; }
        IPCGPURepository PCGPURepository { get; }
        IPCPSURepository PCPSURepository { get; }
        IPCStorageRepository PCStorageRepository { get; }
        IPCNetworkCardRepository PCNetworkCardRepository { get; }
        IPCMonitorRepository PCMonitorRepository { get; }

        Task<bool> Complete();
        bool HasChanges();
    }
}