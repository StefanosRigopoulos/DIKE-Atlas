using API.DTOs;
using API.DTOs.User;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            // AppUser Mapper options
            CreateMap<LoginDTO, AppUser>();
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<AppUser, UserDTO>();

            // Employee Mapper Options
            // Retrival
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.Employee_PCs.Select(employee_pc => employee_pc.PCID).ToList()));
            CreateMap<Employee, EmployeeOnlyDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
            // Registration
            CreateMap<EmployeeDTO, Employee>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // PC Mapper Options
            // Retrival
            CreateMap<PC, PCDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.EmployeeIDs, opt => opt.MapFrom(src => src.Employee_PCs.Select(employee_pc => employee_pc.EmployeeID).ToList()))
                .ForMember(dest => dest.CPUIDs, opt => opt.MapFrom(src => src.PC_CPUs.Select(pc_cpu => pc_cpu.CPUID).ToList()))
                .ForMember(dest => dest.MOBOIDs, opt => opt.MapFrom(src => src.PC_MOBOs.Select(pc_mobo => pc_mobo.MOBOID).ToList()))
                .ForMember(dest => dest.RAMIDs, opt => opt.MapFrom(src => src.PC_RAMs.Select(pc_ram => pc_ram.RAMID).ToList()))
                .ForMember(dest => dest.GPUIDs, opt => opt.MapFrom(src => src.PC_GPUs.Select(pc_gpu => pc_gpu.GPUID).ToList()))
                .ForMember(dest => dest.PSUIDs, opt => opt.MapFrom(src => src.PC_PSUs.Select(pc_psu => pc_psu.PSUID).ToList()))
                .ForMember(dest => dest.StorageIDs, opt => opt.MapFrom(src => src.PC_Storages.Select(pc_storage => pc_storage.StorageID).ToList()))
                .ForMember(dest => dest.NetworkCardIDs, opt => opt.MapFrom(src => src.PC_NetworkCards.Select(pc_networkCard => pc_networkCard.NetworkCardID).ToList()))
                .ForMember(dest => dest.MonitorIDs, opt => opt.MapFrom(src => src.PC_Monitors.Select(pc_monitor => pc_monitor.MonitorID).ToList()));
            CreateMap<PC, PCWithComponentsDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
                .ForMember(dest => dest.CPUs, opt => opt.MapFrom(src => src.PC_CPUs.Select(pc_cpu => pc_cpu.CPU)))
                .ForMember(dest => dest.MOBOs, opt => opt.MapFrom(src => src.PC_MOBOs.Select(pc_mobo => pc_mobo.MOBO)))
                .ForMember(dest => dest.RAMs, opt => opt.MapFrom(src => src.PC_RAMs.Select(pc_ram => pc_ram.RAM)))
                .ForMember(dest => dest.GPUs, opt => opt.MapFrom(src => src.PC_GPUs.Select(pc_gpu => pc_gpu.GPU)))
                .ForMember(dest => dest.PSUs, opt => opt.MapFrom(src => src.PC_PSUs.Select(pc_psu => pc_psu.PSU)))
                .ForMember(dest => dest.Storages, opt => opt.MapFrom(src => src.PC_Storages.Select(pc_storage => pc_storage.Storage)))
                .ForMember(dest => dest.NetworkCards, opt => opt.MapFrom(src => src.PC_NetworkCards.Select(pc_networkCard => pc_networkCard.NetworkCard)))
                .ForMember(dest => dest.Monitors, opt => opt.MapFrom(src => src.PC_Monitors.Select(pc_monitor => pc_monitor.Monitor)));
            CreateMap<PC, PCOnlyDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode));
            // Registration
            CreateMap<PCDTO, PC>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // CPU Mapper Options
            // Retrival
            CreateMap<CPU, CPUDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_CPUs.Select(pc_cpu => pc_cpu.PCID).ToList()));
            // Registration
            CreateMap<CPUDTO, CPU>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // MOBO Mapper Options
            // Retrival
            CreateMap<MOBO, MOBODTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_MOBOs.Select(pc_mobo => pc_mobo.PCID).ToList()));
            // Registration
            CreateMap<MOBODTO, MOBO>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // RAM Mapper OptionsStorage
            // Retrival
            CreateMap<RAM, RAMDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_RAMs.Select(pc_ram => pc_ram.PCID).ToList()));
            // Registration
            CreateMap<RAMDTO, RAM>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // GPU Mapper Options
            // Retrival
            CreateMap<GPU, GPUDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_GPUs.Select(pc_gpu => pc_gpu.PCID).ToList()));
            // Registration
            CreateMap<GPUDTO, GPU>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // PSU Mapper Options
            // Retrival
            CreateMap<PSU, PSUDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_PSUs.Select(pc_psu => pc_psu.PCID).ToList()));
            // Registration
            CreateMap<PSUDTO, PSU>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // Storage Mapper Options
            // Retrival
            CreateMap<Storage, StorageDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_Storages.Select(pc_storage => pc_storage.PCID).ToList()));
            // Registration
            CreateMap<StorageDTO, Storage>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // NetworkAdapter Mapper Options
            // Retrival
            CreateMap<NetworkCard, NetworkCardDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_NetworkCards.Select(pc_networkcard => pc_networkcard.PCID).ToList()));
            // Registration
            CreateMap<NetworkCardDTO, NetworkCard>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            // Monitor Mapper Options
            // Retrival
            CreateMap<Entities.Monitor, MonitorDTO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PCIDs, opt => opt.MapFrom(src => src.PC_Monitors.Select(pc_monitor => pc_monitor.PCID).ToList()));
            // Registration
            CreateMap<MonitorDTO, Entities.Monitor>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());
        }
    }
}