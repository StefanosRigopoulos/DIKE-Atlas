using API.DTOs;
using API.Entities.Junctions;
using API.Entities;
using API.Interfaces.Junctions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PCMonitorRepository(DataContext context, IMapper mapper) : IPCMonitorRepository
    {
        public void Delete(PC_Monitor pc_monitor)
        {
            context.Remove(pc_monitor);
        }
        public async Task AddPCMonitorAsync(PC_Monitor pc_monitor)
        {
            await context.PC_Monitors.AddAsync(pc_monitor);
        }
        public async Task<PC_Monitor?> GetRelationshipAsync(int monitorid, int pcid)
        {
            return await context.PC_Monitors
                                .FirstOrDefaultAsync(x => x.MonitorID == monitorid && x.PCID == pcid);
        }
        public async Task<IEnumerable<PC_Monitor>> GetRelationshipMonitorsAsync(int monitorid)
        {
            return await context.PC_Monitors
                                .Where(x => x.MonitorID == monitorid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_Monitor>> GetRelationshipPCsAsync(int pcid)
        {
            return await context.PC_Monitors
                                .Where(x => x.PCID == pcid)
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC_Monitor>> GetRelationshipsAsync()
        {
            return await context.PC_Monitors
                                .ToListAsync();
        }
        public async Task<IEnumerable<PC>> GetPCsAsync(int monitorid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_Monitors.Any(pc_monitor => pc_monitor.MonitorID == monitorid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<Entities.Monitor>> GetMonitorsAsync(int pcid)
        {
            return await context.Monitors
                                .Where(monitor => monitor.PC_Monitors.Any(pc_monitor => pc_monitor.PCID == pcid))
                                .ToListAsync();
        }
        public async Task<IEnumerable<PCDTO>> GetPCsDTOAsync(int monitorid)
        {
            return await context.PCs
                                .Where(pc => pc.PC_Monitors.Any(pc_monitor => pc_monitor.MonitorID == monitorid))
                                .ProjectTo<PCDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
        public async Task<IEnumerable<MonitorDTO>> GetMonitorsDTOAsync(int pcid)
        {
            return await context.Monitors
                                .Where(monitor => monitor.PC_Monitors.Any(pc_monitor => pc_monitor.PCID == pcid))
                                .ProjectTo<MonitorDTO>(mapper.ConfigurationProvider)
                                .ToListAsync();
        }
    }
}