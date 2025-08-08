using API.DTOs;
using API.Entities;
using API.Entities.Junctions;
using API.Helpers;
using API.Interfaces;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace API.Data
{
    public class FileRepository(DataContext context) : IFileRepository
    {
        // Import methods
        public async Task<bool> ImportDBAsync(DataModel data)
        {
            if (data == null) return false;
            Console.WriteLine("We have data");
            // Remove existing data (optional)
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Employees\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PCs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"CPUs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"MOBOs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RAMs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"GPUs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PSUs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Storages\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"NetworkCards\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Monitors\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Employee_PCs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_CPUs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_MOBOs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_RAMs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_GPUs\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_Storages\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_NetworkCards\" RESTART IDENTITY CASCADE");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PC_Monitors\" RESTART IDENTITY CASCADE");
            await context.SaveChangesAsync();
            // Add new data
            var employees = data.Employees.Select(employee => new Employee
            {
                ID = employee.ID,
                Rank = employee.Rank,
                Speciality = employee.Speciality,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                AM = employee.AM,
                Unit = employee.Unit,
                Office = employee.Office,
                Position = employee.Position,
                PCUsername = employee.PCUsername,
                SHDAEDUsername = employee.SHDAEDUsername,
                Phone = employee.Phone,
                Mobile = employee.Mobile,
                Email = employee.Email,
                PhotoPath = employee.PhotoPath
            }).ToList();
            var pcs = data.PCs.Select(pc => new PC
            {
                ID = pc.ID,
                Barcode = pc.Barcode,
                SerialNumber = pc.SerialNumber,
                Brand = pc.Brand,
                Model = pc.Model,
                PCName = pc.PCName,
                AdministratorCode = pc.AdministratorCode,
                BIOSCode = pc.BIOSCode,
                PDFReportPath = pc.PDFReportPath,
                Domain = pc.Domain,
                IP = pc.IP,
                ExternalIP = pc.ExternalIP,
                SubnetMask = pc.SubnetMask,
                Gateway = pc.Gateway,
                DNS1 = pc.DNS1,
                DNS2 = pc.DNS2
            }).ToList();
            var cpus = data.CPUs.Select(cpu => new CPU
            {
                ID = cpu.ID,
                Barcode = cpu.Barcode,
                SerialNumber = cpu.SerialNumber,
                Brand = cpu.Brand,
                Model = cpu.Model,
                Cores = cpu.Cores,
                Threads = cpu.Threads,
                Specification = cpu.Specification,
                Package = cpu.Package,
                Chipset = cpu.Chipset,
                IntegratedGPUModel = cpu.IntegratedGPUModel
            }).ToList();
            var mobos = data.MOBOs.Select(mobo => new MOBO
            {
                ID = mobo.ID,
                Barcode = mobo.Barcode,
                SerialNumber = mobo.SerialNumber,
                Brand = mobo.Brand,
                Model = mobo.Model,
                DRAMSlots = mobo.DRAMSlots,
                ChipsetModel = mobo.ChipsetModel,
                ChipsetVendor = mobo.ChipsetVendor,
                BIOSBrand = mobo.BIOSBrand,
                BIOSVersion = mobo.BIOSVersion,
                OnBoardGPUBrand = mobo.OnBoardGPUBrand,
                OnBoardGPUModel = mobo.OnBoardGPUModel,
                OnBoardNetworkAdapterBrand = mobo.OnBoardNetworkAdapterBrand,
                OnBoardNetworkAdapterModel = mobo.OnBoardNetworkAdapterModel
            }).ToList();
            var rams = data.RAMs.Select(ram => new RAM
            {
                ID = ram.ID,
                Barcode = ram.Barcode,
                SerialNumber = ram.SerialNumber,
                Brand = ram.Brand,
                Model = ram.Model,
                Type = ram.Type,
                Size = ram.Size,
                Frequency = ram.Frequency,
                CASLatency = ram.CASLatency
            }).ToList();
            var gpus = data.GPUs.Select(gpu => new GPU
            {
                ID = gpu.ID,
                Barcode = gpu.Barcode,
                SerialNumber = gpu.SerialNumber,
                Brand = gpu.Brand,
                Model = gpu.Model,
                Memory = gpu.Memory,
                DriverVersion = gpu.DriverVersion
            }).ToList();
            var psus = data.PSUs.Select(psu => new PSU
            {
                ID = psu.ID,
                Barcode = psu.Barcode,
                SerialNumber = psu.SerialNumber,
                Brand = psu.Brand,
                Model = psu.Model,
                Wattage = psu.Wattage,
                Certification = psu.Certification
            }).ToList();
            var storages = data.Storages.Select(storage => new Storage
            {
                ID = storage.ID,
                Barcode = storage.Barcode,
                SerialNumber = storage.SerialNumber,
                Brand = storage.Brand,
                Model = storage.Model,
                Type = storage.Type,
                Interface = storage.Interface,
                Speed = storage.Speed,
                Capacity = storage.Capacity
            }).ToList();
            var networkcards = data.NetworkCards.Select(networkcard => new NetworkCard
            {
                ID = networkcard.ID,
                Barcode = networkcard.Barcode,
                SerialNumber = networkcard.SerialNumber,
                Brand = networkcard.Brand,
                Model = networkcard.Model
            }).ToList();
            var monitors = data.Monitors.Select(monitor => new Entities.Monitor
            {
                ID = monitor.ID,
                Barcode = monitor.Barcode,
                SerialNumber = monitor.SerialNumber,
                Brand = monitor.Brand,
                Model = monitor.Model,
                Resolution = monitor.Resolution,
                Inches = monitor.Inches
            }).ToList();
            await context.Employees.AddRangeAsync(employees);
            await context.PCs.AddRangeAsync(pcs);
            await context.CPUs.AddRangeAsync(cpus);
            await context.MOBOs.AddRangeAsync(mobos);
            await context.RAMs.AddRangeAsync(rams);
            await context.GPUs.AddRangeAsync(gpus);
            await context.PSUs.AddRangeAsync(psus);
            await context.Storages.AddRangeAsync(storages);
            await context.NetworkCards.AddRangeAsync(networkcards);
            await context.Monitors.AddRangeAsync(monitors);
            await context.SaveChangesAsync();
            // Restore Relationships
            var employee_pcs = new List<Employee_PC>();
            foreach (var employeeDTO in data.Employees)
            {
                foreach (var pcID in employeeDTO.PCIDs)
                {
                    employee_pcs.Add(new Employee_PC
                    {
                        EmployeeID = employeeDTO.ID,
                        PCID = pcID
                    });
                }
            }
            var pc_cpus = new List<PC_CPU>();
            foreach (var cpu in data.CPUs)
            {
                foreach (var pc_id in cpu.PCIDs)
                {
                    pc_cpus.Add(new PC_CPU
                    {
                        PCID = pc_id,
                        CPUID = cpu.ID
                    });
                }
            }
            var pc_mobos = new List<PC_MOBO>();
            foreach (var mobo in data.MOBOs)
            {
                foreach (var pc_id in mobo.PCIDs)
                {
                    pc_mobos.Add(new PC_MOBO
                    {
                        PCID = pc_id,
                        MOBOID = mobo.ID
                    });
                }
            }
            var pc_rams = new List<PC_RAM>();
            foreach (var ram in data.RAMs)
            {
                foreach (var pc_id in ram.PCIDs)
                {
                    pc_rams.Add(new PC_RAM
                    {
                        PCID = pc_id,
                        RAMID = ram.ID
                    });
                }
            }
            var pc_gpus = new List<PC_GPU>();
            foreach (var gpu in data.GPUs)
            {
                foreach (var pc_id in gpu.PCIDs)
                {
                    pc_gpus.Add(new PC_GPU
                    {
                        PCID = pc_id,
                        GPUID = gpu.ID
                    });
                }
            }
            var pc_psus = new List<PC_PSU>();
            foreach (var psu in data.PSUs)
            {
                foreach (var pc_id in psu.PCIDs)
                {
                    pc_psus.Add(new PC_PSU
                    {
                        PCID = pc_id,
                        PSUID = psu.ID
                    });
                }
            }
            var pc_storages = new List<PC_Storage>();
            foreach (var storage in data.Storages)
            {
                foreach (var pc_id in storage.PCIDs)
                {
                    pc_storages.Add(new PC_Storage
                    {
                        PCID = pc_id,
                        StorageID = storage.ID
                    });
                }
            }
            var pc_networkcards = new List<PC_NetworkCard>();
            foreach (var networkcard in data.NetworkCards)
            {
                foreach (var pc_id in networkcard.PCIDs)
                {
                    pc_networkcards.Add(new PC_NetworkCard
                    {
                        PCID = pc_id,
                        NetworkCardID = networkcard.ID
                    });
                }
            }
            var pc_monitors = new List<PC_Monitor>();
            foreach (var monitor in data.Monitors)
            {
                foreach (var pc_id in monitor.PCIDs)
                {
                    pc_monitors.Add(new PC_Monitor
                    {
                        PCID = pc_id,
                        MonitorID = monitor.ID
                    });
                }
            }
            await context.Employee_PCs.AddRangeAsync(employee_pcs);
            await context.PC_CPUs.AddRangeAsync(pc_cpus);
            await context.PC_MOBOs.AddRangeAsync(pc_mobos);
            await context.PC_RAMs.AddRangeAsync(pc_rams);
            await context.PC_GPUs.AddRangeAsync(pc_gpus);
            await context.PC_PSUs.AddRangeAsync(pc_psus);
            await context.PC_Storages.AddRangeAsync(pc_storages);
            await context.PC_NetworkCards.AddRangeAsync(pc_networkcards);
            await context.PC_Monitors.AddRangeAsync(pc_monitors);
            await context.SaveChangesAsync();
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Employees\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"Employees\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PCs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"PCs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"CPUs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"CPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"MOBOs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"MOBOs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"RAMs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"RAMs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"GPUs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"GPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PSUs\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"PSUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Storages\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"Storages\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"NetworkCards\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"NetworkCards\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Monitors\"', 'ID'), COALESCE(MAX(\"ID\"), 1) + 1, false) FROM \"Monitors\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Employee_PCs\"', 'EmployeeID'), COALESCE(MAX(\"EmployeeID\"), 1) + 1, false) FROM \"Employee_PCs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"Employee_PCs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"Employee_PCs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_CPUs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_CPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_CPUs\"', 'CPUID'), COALESCE(MAX(\"CPUID\"), 1) + 1, false) FROM \"PC_CPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_MOBOs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_MOBOs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_MOBOs\"', 'MOBOID'), COALESCE(MAX(\"MOBOID\"), 1) + 1, false) FROM \"PC_MOBOs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_RAMs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_RAMs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_RAMs\"', 'RAMID'), COALESCE(MAX(\"RAMID\"), 1) + 1, false) FROM \"PC_RAMs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_GPUs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_GPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_GPUs\"', 'GPUID'), COALESCE(MAX(\"GPUID\"), 1) + 1, false) FROM \"PC_GPUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_PSUs\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_PSUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_PSUs\"', 'PSUID'), COALESCE(MAX(\"PSUID\"), 1) + 1, false) FROM \"PC_PSUs\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_Storages\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_Storages\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_Storages\"', 'StorageID'), COALESCE(MAX(\"StorageID\"), 1) + 1, false) FROM \"PC_Storages\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_NetworkCards\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_NetworkCards\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_NetworkCards\"', 'NetworkCardID'), COALESCE(MAX(\"NetworkCardID\"), 1) + 1, false) FROM \"PC_NetworkCards\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_Monitors\"', 'PCID'), COALESCE(MAX(\"PCID\"), 1) + 1, false) FROM \"PC_Monitors\";");
            await context.Database.ExecuteSqlRawAsync("SELECT setval(pg_get_serial_sequence('\"PC_Monitors\"', 'MonitorID'), COALESCE(MAX(\"MonitorID\"), 1) + 1, false) FROM \"PC_Monitors\";");
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ImportEmployeeTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<Employee> records = new();
            for (int row = 2; row <= rows; row++)
            {
                var record = new Employee
                {
                    Rank = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                    Speciality = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                    FirstName = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                    LastName = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                    AM = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                    Unit = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                    Office = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                    Position = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                    PCUsername = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                    SHDAEDUsername = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                    Phone = worksheet.Cells[row, 11].Value?.ToString() ?? "",
                    Mobile = worksheet.Cells[row, 12].Value?.ToString() ?? "",
                    Email = worksheet.Cells[row, 13].Value?.ToString() ?? ""
                };
                bool check = await context.Employees.AnyAsync(x => x.Rank == record.Rank && x.FirstName == record.FirstName &&
                                                                   x.LastName == record.LastName && x.Unit == record.Unit &&
                                                                   x.Office == record.Office && x.Position == record.Position);
                if (!check) {
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.Employees.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportPCTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<PC> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 1].Value?.ToString() == null) {
                    var record = new PC
                    {
                        Barcode = BarcodeGenerator.GenerateBarcode("PC"),
                        SerialNumber = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        PCName = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        BIOSCode = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        AdministratorCode = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Domain = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        IP = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                        ExternalIP = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                        SubnetMask = worksheet.Cells[row, 11].Value?.ToString() ?? "",
                        Gateway = worksheet.Cells[row, 12].Value?.ToString() ?? "",
                        DNS1 = worksheet.Cells[row, 13].Value?.ToString() ?? "",
                        DNS2 = worksheet.Cells[row, 14].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.PCs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportCPUTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<CPU> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new CPU
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("CPU"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Cores = worksheet.Cells[row, 5].Value != null ? Convert.ToInt32(worksheet.Cells[row, 5].Value) : 0,
                        Threads = worksheet.Cells[row, 6].Value != null ? Convert.ToInt32(worksheet.Cells[row, 6].Value) : 0,
                        Specification = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Package = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        Chipset = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                        IntegratedGPUModel = worksheet.Cells[row, 10].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.CPUs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportMOBOTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<MOBO> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new MOBO
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("MOBO"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        DRAMSlots = worksheet.Cells[row, 5].Value != null ? Convert.ToInt32(worksheet.Cells[row, 5].Value) : 0,
                        ChipsetVendor = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        ChipsetModel = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        BIOSBrand = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        BIOSVersion = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                        OnBoardGPUBrand = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                        OnBoardGPUModel = worksheet.Cells[row, 11].Value?.ToString() ?? "",
                        OnBoardNetworkAdapterBrand = worksheet.Cells[row, 12].Value?.ToString() ?? "",
                        OnBoardNetworkAdapterModel = worksheet.Cells[row, 13].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.MOBOs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportRAMTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<RAM> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new RAM
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("RAM"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Type = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Size = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Frequency = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        CASLatency = worksheet.Cells[row, 8].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.RAMs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportGPUTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<GPU> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new GPU
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("GPU"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Memory = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        DriverVersion = worksheet.Cells[row, 6].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.GPUs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportPSUTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<PSU> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new PSU
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("PSU"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Wattage = worksheet.Cells[row, 5].Value != null ? Convert.ToInt32(worksheet.Cells[row, 5].Value) : 0,
                        Certification = worksheet.Cells[row, 6].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.PSUs.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportStorageTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<Storage> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new Storage
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("STOR"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Type = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Interface = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Speed = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Capacity = worksheet.Cells[row, 8].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.Storages.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportNetworkCardTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<NetworkCard> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new NetworkCard
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("NET"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? ""
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.NetworkCards.AddRangeAsync(records);
                return true;
            }
            return false;
        }
        public async Task<bool> ImportMonitorTableAsync(ExcelWorksheet worksheet, int rows)
        {
            List<Entities.Monitor> records = new();
            for (int row = 2; row <= rows; row++)
            {
                if (worksheet.Cells[row, 2].Value?.ToString() == null) {
                    var record = new Entities.Monitor
                    {
                        SerialNumber = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        Barcode = BarcodeGenerator.GenerateBarcode("MONI"),
                        Brand = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        Model = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        Resolution = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Inches = worksheet.Cells[row, 6].Value != null ? Convert.ToInt32(worksheet.Cells[row, 6].Value) : 0
                    };
                    records.Add(record);
                }
            }
            if (!records.IsNullOrEmpty()) {
                await context.Monitors.AddRangeAsync(records);
                return true;
            }
            return false;
        }

        // Export methods
        public async Task<DataModel> ExportDBAsync()
        {
            var data = new DataModel
            {
                Employees = await context.Employees
                    .Select(employee => new EmployeeDTO
                    {
                        ID = employee.ID,
                        Rank = employee.Rank,
                        Speciality = employee.Speciality,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        AM = employee.AM,
                        Unit = employee.Unit,
                        Office = employee.Office,
                        Position = employee.Position,
                        PCUsername = employee.PCUsername,
                        SHDAEDUsername = employee.SHDAEDUsername,
                        Phone = employee.Phone,
                        Mobile = employee.Mobile,
                        Email = employee.Email,
                        PhotoPath = employee.PhotoPath,
                        PCIDs = employee.Employee_PCs.Select(employee_pc => employee_pc.PCID).ToList()
                    })
                    .ToListAsync(),
                PCs = await context.PCs
                    .Select(pc => new PCDTO
                    {
                        ID = pc.ID,
                        Barcode = pc.Barcode,
                        SerialNumber = pc.SerialNumber,
                        Brand = pc.Brand,
                        Model = pc.Model,
                        PCName = pc.PCName,
                        AdministratorCode = pc.AdministratorCode,
                        BIOSCode = pc.BIOSCode,
                        PDFReportPath = pc.PDFReportPath,
                        Domain = pc.Domain,
                        IP = pc.IP,
                        ExternalIP = pc.ExternalIP,
                        SubnetMask = pc.SubnetMask,
                        Gateway = pc.Gateway,
                        DNS1 = pc.DNS1,
                        DNS2 = pc.DNS2,
                        EmployeeIDs = pc.Employee_PCs.Select(employee_pc => employee_pc.EmployeeID).ToList(),
                        CPUIDs = pc.PC_CPUs.Select(pc_cpu => pc_cpu.CPUID).ToList(),
                        MOBOIDs = pc.PC_MOBOs.Select(pc_mobo => pc_mobo.MOBOID).ToList(),
                        RAMIDs = pc.PC_RAMs.Select(pc_ram => pc_ram.RAMID).ToList(),
                        GPUIDs = pc.PC_GPUs.Select(pc_gpu => pc_gpu.GPUID).ToList(),
                        PSUIDs = pc.PC_PSUs.Select(pc_psu => pc_psu.PSUID).ToList(),
                        StorageIDs = pc.PC_Storages.Select(pc_storage => pc_storage.StorageID).ToList(),
                        NetworkCardIDs = pc.PC_NetworkCards.Select(pc_networkcard => pc_networkcard.NetworkCardID).ToList(),
                        MonitorIDs = pc.PC_Monitors.Select(pc_monitor => pc_monitor.MonitorID).ToList()
                    })
                    .ToListAsync(),
                CPUs = await context.CPUs
                    .Select(cpu => new CPUDTO
                    {
                        ID = cpu.ID,
                        Barcode = cpu.Barcode,
                        SerialNumber = cpu.SerialNumber,
                        Brand = cpu.Brand,
                        Model = cpu.Model,
                        Cores = cpu.Cores,
                        Threads = cpu.Threads,
                        Specification = cpu.Specification,
                        Package = cpu.Package,
                        Chipset = cpu.Chipset,
                        IntegratedGPUModel = cpu.IntegratedGPUModel,
                        PCIDs = cpu.PC_CPUs.Select(pc_cpu => pc_cpu.PCID).ToList()
                    })
                    .ToListAsync(),
                MOBOs = await context.MOBOs
                    .Select(mobo => new MOBODTO
                    {
                        ID = mobo.ID,
                        Barcode = mobo.Barcode,
                        SerialNumber = mobo.SerialNumber,
                        Brand = mobo.Brand,
                        Model = mobo.Model,
                        DRAMSlots = mobo.DRAMSlots,
                        ChipsetModel = mobo.ChipsetModel,
                        ChipsetVendor = mobo.ChipsetVendor,
                        BIOSBrand = mobo.BIOSBrand,
                        BIOSVersion = mobo.BIOSVersion,
                        OnBoardGPUBrand = mobo.OnBoardGPUBrand,
                        OnBoardGPUModel = mobo.OnBoardGPUModel,
                        OnBoardNetworkAdapterBrand = mobo.OnBoardNetworkAdapterBrand,
                        OnBoardNetworkAdapterModel = mobo.OnBoardNetworkAdapterModel,
                        PCIDs = mobo.PC_MOBOs.Select(pc_mobo => pc_mobo.PCID).ToList()
                    })
                    .ToListAsync(),
                RAMs = await context.RAMs
                    .Select(ram => new RAMDTO
                    {
                        ID = ram.ID,
                        Barcode = ram.Barcode,
                        SerialNumber = ram.SerialNumber,
                        Brand = ram.Brand,
                        Model = ram.Model,
                        Type = ram.Type,
                        Size = ram.Size,
                        Frequency = ram.Frequency,
                        CASLatency = ram.CASLatency,
                        PCIDs = ram.PC_RAMs.Select(pc_ram => pc_ram.PCID).ToList()
                    })
                    .ToListAsync(),
                GPUs = await context.GPUs
                    .Select(gpu => new GPUDTO
                    {
                        ID = gpu.ID,
                        Barcode = gpu.Barcode,
                        SerialNumber = gpu.SerialNumber,
                        Brand = gpu.Brand,
                        Model = gpu.Model,
                        Memory = gpu.Memory,
                        DriverVersion = gpu.DriverVersion,
                        PCIDs = gpu.PC_GPUs.Select(pc_gpu => pc_gpu.PCID).ToList()
                    })
                    .ToListAsync(),
                PSUs = await context.PSUs
                    .Select(psu => new PSUDTO
                    {
                        ID = psu.ID,
                        Barcode = psu.Barcode,
                        SerialNumber = psu.SerialNumber,
                        Brand = psu.Brand,
                        Model = psu.Model,
                        Wattage = psu.Wattage,
                        Certification = psu.Certification,
                        PCIDs = psu.PC_PSUs.Select(pc_psu => pc_psu.PCID).ToList()
                    })
                    .ToListAsync(),
                Storages = await context.Storages
                    .Select(storage => new StorageDTO
                    {
                        ID = storage.ID,
                        Barcode = storage.Barcode,
                        SerialNumber = storage.SerialNumber,
                        Brand = storage.Brand,
                        Model = storage.Model,
                        Type = storage.Type,
                        Interface = storage.Interface,
                        Speed = storage.Speed,
                        Capacity = storage.Capacity,
                        PCIDs = storage.PC_Storages.Select(pc_storage => pc_storage.PCID).ToList()
                    })
                    .ToListAsync(),
                NetworkCards = await context.NetworkCards
                    .Select(networkcard => new NetworkCardDTO
                    {
                        ID = networkcard.ID,
                        Barcode = networkcard.Barcode,
                        SerialNumber = networkcard.SerialNumber,
                        Brand = networkcard.Brand,
                        Model = networkcard.Model,
                        PCIDs = networkcard.PC_NetworkCards.Select(pc_networkcard => pc_networkcard.PCID).ToList()
                    })
                    .ToListAsync(),
                Monitors = await context.Monitors
                    .Select(monitor => new MonitorDTO
                    {
                        ID = monitor.ID,
                        Barcode = monitor.Barcode,
                        SerialNumber = monitor.SerialNumber,
                        Brand = monitor.Brand,
                        Model = monitor.Model,
                        Resolution = monitor.Resolution,
                        Inches = monitor.Inches,
                        PCIDs = monitor.PC_Monitors.Select(pc_monitor => pc_monitor.PCID).ToList()
                    })
                    .ToListAsync()
            };
            return data;
        }
        public async Task<MemoryStream> ExportDatabaseAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            int row;
            // Employees
            var employees = await context.Employees.ToListAsync();
            var employeesWorksheet = workbook.Worksheets.Add("Στελέχοι");
            employeesWorksheet.Cell(1, 1).Value = "Βαθμός";
            employeesWorksheet.Cell(1, 2).Value = "Όπλο/Σώμα";
            employeesWorksheet.Cell(1, 3).Value = "Όνομα";
            employeesWorksheet.Cell(1, 4).Value = "Επίθετο";
            employeesWorksheet.Cell(1, 5).Value = "Αριθμός Μητρώου";
            employeesWorksheet.Cell(1, 6).Value = "Μονάδα";
            employeesWorksheet.Cell(1, 7).Value = "Γραφείο";
            employeesWorksheet.Cell(1, 8).Value = "Θέση";
            employeesWorksheet.Cell(1, 9).Value = "Όνομα Χρήστη Η/Υ";
            employeesWorksheet.Cell(1, 10).Value = "Όνομα Χρήστη ΣΗΔΑΕΔ";
            employeesWorksheet.Cell(1, 11).Value = "Τηλέφωνο";
            employeesWorksheet.Cell(1, 12).Value = "Κινητό";
            employeesWorksheet.Cell(1, 13).Value = "Email";
            row = 2;
            foreach (var employee in employees)
            {
                employeesWorksheet.Cell(row, 1).Value = employee.Rank;
                employeesWorksheet.Cell(row, 2).Value = employee.Speciality;
                employeesWorksheet.Cell(row, 3).Value = employee.FirstName;
                employeesWorksheet.Cell(row, 4).Value = employee.LastName;
                employeesWorksheet.Cell(row, 5).Value = employee.AM;
                employeesWorksheet.Cell(row, 6).Value = employee.Unit;
                employeesWorksheet.Cell(row, 7).Value = employee.Office;
                employeesWorksheet.Cell(row, 8).Value = employee.Position;
                employeesWorksheet.Cell(row, 9).Value = employee.PCUsername;
                employeesWorksheet.Cell(row, 10).Value = employee.SHDAEDUsername;
                employeesWorksheet.Cell(row, 11).Value = employee.Phone;
                employeesWorksheet.Cell(row, 12).Value = employee.Mobile;
                employeesWorksheet.Cell(row, 13).Value = employee.Email;
                row++;
            }
            // PCs
            var pcs = await context.PCs.ToListAsync();
            var pcsWorksheet = workbook.Worksheets.Add("Ηλεκτονικοί Υπολογιστές");
            pcsWorksheet.Cell(1, 1).Value = "Barcode";
            pcsWorksheet.Cell(1, 2).Value = "Σεριακός Αριθμός";
            pcsWorksheet.Cell(1, 3).Value = "Μάρκα";
            pcsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            pcsWorksheet.Cell(1, 5).Value = "Όνομα Η/Υ";
            pcsWorksheet.Cell(1, 6).Value = "Κωδικός Διαχειριστή";
            pcsWorksheet.Cell(1, 7).Value = "Κωδικός BIOS";
            pcsWorksheet.Cell(1, 8).Value = "Τομέας";
            pcsWorksheet.Cell(1, 9).Value = "Διεύθυνση IP";
            pcsWorksheet.Cell(1, 10).Value = "Εξωτερική IP";
            pcsWorksheet.Cell(1, 11).Value = "Μάσκα Δικτύου";
            pcsWorksheet.Cell(1, 12).Value = "Πύλη";
            pcsWorksheet.Cell(1, 13).Value = "DNS1";
            pcsWorksheet.Cell(1, 14).Value = "DNS2";
            row = 2;
            foreach (var pc in pcs)
            {
                pcsWorksheet.Cell(row, 1).Value = pc.Barcode;
                pcsWorksheet.Cell(row, 2).Value = pc.SerialNumber;
                pcsWorksheet.Cell(row, 3).Value = pc.Brand;
                pcsWorksheet.Cell(row, 4).Value = pc.Model;
                pcsWorksheet.Cell(row, 5).Value = pc.PCName;
                pcsWorksheet.Cell(row, 6).Value = pc.AdministratorCode;
                pcsWorksheet.Cell(row, 7).Value = pc.BIOSCode;
                pcsWorksheet.Cell(row, 8).Value = pc.Domain;
                pcsWorksheet.Cell(row, 9).Value = pc.IP;
                pcsWorksheet.Cell(row, 10).Value = pc.ExternalIP;
                pcsWorksheet.Cell(row, 11).Value = pc.SubnetMask;
                pcsWorksheet.Cell(row, 12).Value = pc.Gateway;
                pcsWorksheet.Cell(row, 13).Value = pc.DNS1;
                pcsWorksheet.Cell(row, 14).Value = pc.DNS2;
                row++;
            }
            // CPUs
            var cpus = await context.CPUs.ToListAsync();
            var cpusWorksheet = workbook.Worksheets.Add("Επεξεργαστές");
            cpusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            cpusWorksheet.Cell(1, 2).Value = "Barcode";
            cpusWorksheet.Cell(1, 3).Value = "Μάρκα";
            cpusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            cpusWorksheet.Cell(1, 5).Value = "Πυρήνες";
            cpusWorksheet.Cell(1, 6).Value = "Νήματα";
            cpusWorksheet.Cell(1, 7).Value = "Χαρακτηριστηκά";
            cpusWorksheet.Cell(1, 8).Value = "Πακέτο";
            cpusWorksheet.Cell(1, 9).Value = "Chipset";
            cpusWorksheet.Cell(1, 10).Value = "Ενσωματωμένα Γραφικά";
            row = 2;
            foreach (var cpu in cpus)
            {
                cpusWorksheet.Cell(row, 1).Value = cpu.SerialNumber;
                cpusWorksheet.Cell(row, 2).Value = cpu.Barcode;
                cpusWorksheet.Cell(row, 3).Value = cpu.Brand;
                cpusWorksheet.Cell(row, 4).Value = cpu.Model;
                cpusWorksheet.Cell(row, 5).Value = cpu.Cores;
                cpusWorksheet.Cell(row, 6).Value = cpu.Threads;
                cpusWorksheet.Cell(row, 7).Value = cpu.Specification;
                cpusWorksheet.Cell(row, 8).Value = cpu.Package;
                cpusWorksheet.Cell(row, 9).Value = cpu.Chipset;
                cpusWorksheet.Cell(row, 10).Value = cpu.IntegratedGPUModel;
                row++;
            }
            // MOBOs
            var mobos = await context.MOBOs.ToListAsync();
            var mobosWorksheet = workbook.Worksheets.Add("Μητρικές Κάρτες");
            mobosWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            mobosWorksheet.Cell(1, 2).Value = "Barcode";
            mobosWorksheet.Cell(1, 3).Value = "Μάρκα";
            mobosWorksheet.Cell(1, 4).Value = "Μοντέλο";
            mobosWorksheet.Cell(1, 5).Value = "Θέσεις Μνημών";
            mobosWorksheet.Cell(1, 6).Value = "Chipset Μάρκα";
            mobosWorksheet.Cell(1, 7).Value = "Chipset Μοντέλο";
            mobosWorksheet.Cell(1, 8).Value = "BIOS Μάρκα";
            mobosWorksheet.Cell(1, 9).Value = "BIOS Έκδοση";
            mobosWorksheet.Cell(1, 10).Value = "GPU Μάρκα";
            mobosWorksheet.Cell(1, 11).Value = "GPU Μοντέλο";
            mobosWorksheet.Cell(1, 12).Value = "Network Μάρκα";
            mobosWorksheet.Cell(1, 13).Value = "Network Μοντέλο";
            row = 2;
            foreach (var mobo in mobos)
            {
                mobosWorksheet.Cell(row, 1).Value = mobo.SerialNumber;
                mobosWorksheet.Cell(row, 2).Value = mobo.Barcode;
                mobosWorksheet.Cell(row, 3).Value = mobo.Brand;
                mobosWorksheet.Cell(row, 4).Value = mobo.Model;
                mobosWorksheet.Cell(row, 5).Value = mobo.DRAMSlots;
                mobosWorksheet.Cell(row, 6).Value = mobo.ChipsetVendor;
                mobosWorksheet.Cell(row, 7).Value = mobo.ChipsetModel;
                mobosWorksheet.Cell(row, 8).Value = mobo.BIOSBrand;
                mobosWorksheet.Cell(row, 9).Value = mobo.BIOSVersion;
                mobosWorksheet.Cell(row, 10).Value = mobo.OnBoardGPUBrand;
                mobosWorksheet.Cell(row, 11).Value = mobo.OnBoardGPUModel;
                mobosWorksheet.Cell(row, 12).Value = mobo.OnBoardNetworkAdapterBrand;
                mobosWorksheet.Cell(row, 13).Value = mobo.OnBoardNetworkAdapterModel;
                row++;
            }
            // RAMs
            var rams = await context.RAMs.ToListAsync();
            var ramsWorksheet = workbook.Worksheets.Add("Μνήμες");
            ramsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            ramsWorksheet.Cell(1, 2).Value = "Barcode";
            ramsWorksheet.Cell(1, 3).Value = "Μάρκα";
            ramsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            ramsWorksheet.Cell(1, 5).Value = "Τύπος";
            ramsWorksheet.Cell(1, 6).Value = "Μέγεθος";
            ramsWorksheet.Cell(1, 7).Value = "Συχνότητα";
            ramsWorksheet.Cell(1, 8).Value = "CAS Latency";
            row = 2;
            foreach (var ram in rams)
            {
                ramsWorksheet.Cell(row, 1).Value = ram.SerialNumber;
                ramsWorksheet.Cell(row, 2).Value = ram.Barcode;
                ramsWorksheet.Cell(row, 3).Value = ram.Brand;
                ramsWorksheet.Cell(row, 4).Value = ram.Model;
                ramsWorksheet.Cell(row, 5).Value = ram.Type;
                ramsWorksheet.Cell(row, 6).Value = ram.Size;
                ramsWorksheet.Cell(row, 7).Value = ram.Frequency;
                ramsWorksheet.Cell(row, 8).Value = ram.CASLatency;
                row++;
            }
            // GPUs
            var gpus = await context.GPUs.ToListAsync();
            var gpusWorksheet = workbook.Worksheets.Add("Κάρτες Γραφικών");
            gpusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            gpusWorksheet.Cell(1, 2).Value = "Barcode";
            gpusWorksheet.Cell(1, 3).Value = "Μάρκα";
            gpusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            gpusWorksheet.Cell(1, 5).Value = "Μνήμη";
            gpusWorksheet.Cell(1, 6).Value = "Driver Έκδοση";
            row = 2;
            foreach (var gpu in gpus)
            {
                gpusWorksheet.Cell(row, 1).Value = gpu.SerialNumber;
                gpusWorksheet.Cell(row, 2).Value = gpu.Barcode;
                gpusWorksheet.Cell(row, 3).Value = gpu.Brand;
                gpusWorksheet.Cell(row, 4).Value = gpu.Model;
                gpusWorksheet.Cell(row, 5).Value = gpu.Memory;
                gpusWorksheet.Cell(row, 6).Value = gpu.DriverVersion;
                row++;
            }
            // PSUs
            var psus = await context.PSUs.ToListAsync();
            var psusWorksheet = workbook.Worksheets.Add("Τροφοδορικά");
            psusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            psusWorksheet.Cell(1, 2).Value = "Barcode";
            psusWorksheet.Cell(1, 3).Value = "Μάρκα";
            psusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            psusWorksheet.Cell(1, 5).Value = "Ισχύς";
            psusWorksheet.Cell(1, 6).Value = "Πιστοποίηση";
            row = 2;
            foreach (var psu in psus)
            {
                psusWorksheet.Cell(row, 1).Value = psu.SerialNumber;
                psusWorksheet.Cell(row, 2).Value = psu.Barcode;
                psusWorksheet.Cell(row, 3).Value = psu.Brand;
                psusWorksheet.Cell(row, 4).Value = psu.Model;
                psusWorksheet.Cell(row, 5).Value = psu.Wattage;
                psusWorksheet.Cell(row, 6).Value = psu.Certification;
                row++;
            }
            // Storages
            var storages = await context.Storages.ToListAsync();
            var storagesWorksheet = workbook.Worksheets.Add("Αποθηκευτικά Μέσα");
            storagesWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            storagesWorksheet.Cell(1, 2).Value = "Barcode";
            storagesWorksheet.Cell(1, 3).Value = "Μάρκα";
            storagesWorksheet.Cell(1, 4).Value = "Μοντέλο";
            storagesWorksheet.Cell(1, 5).Value = "Τύπος";
            storagesWorksheet.Cell(1, 6).Value = "Διεπαφή";
            storagesWorksheet.Cell(1, 7).Value = "Ταχύτητα";
            storagesWorksheet.Cell(1, 8).Value = "Χωρητικότητα";
            row = 2;
            foreach (var storage in storages)
            {
                storagesWorksheet.Cell(row, 1).Value = storage.SerialNumber;
                storagesWorksheet.Cell(row, 2).Value = storage.Barcode;
                storagesWorksheet.Cell(row, 3).Value = storage.Brand;
                storagesWorksheet.Cell(row, 4).Value = storage.Model;
                storagesWorksheet.Cell(row, 5).Value = storage.Type;
                storagesWorksheet.Cell(row, 6).Value = storage.Interface;
                storagesWorksheet.Cell(row, 7).Value = storage.Speed;
                storagesWorksheet.Cell(row, 8).Value = storage.Capacity;
                row++;
            }
            // NetworkCards
            var networkcards = await context.NetworkCards.ToListAsync();
            var networkcardsWorksheet = workbook.Worksheets.Add("Κάρτες Δικτύου");
            networkcardsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            networkcardsWorksheet.Cell(1, 2).Value = "Barcode";
            networkcardsWorksheet.Cell(1, 3).Value = "Μάρκα";
            networkcardsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            row = 2;
            foreach (var networkcard in networkcards)
            {
                networkcardsWorksheet.Cell(row, 1).Value = networkcard.SerialNumber;
                networkcardsWorksheet.Cell(row, 2).Value = networkcard.Barcode;
                networkcardsWorksheet.Cell(row, 3).Value = networkcard.Brand;
                networkcardsWorksheet.Cell(row, 4).Value = networkcard.Model;
                row++;
            }
            // Monitors
            var monitors = await context.Monitors.ToListAsync();
            var monitorsWorksheet = workbook.Worksheets.Add("Οθόνες");
            monitorsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            monitorsWorksheet.Cell(1, 2).Value = "Barcode";
            monitorsWorksheet.Cell(1, 3).Value = "Μάρκα";
            monitorsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            monitorsWorksheet.Cell(1, 5).Value = "Ανάλυση";
            monitorsWorksheet.Cell(1, 6).Value = "Ίντσες";
            row = 2;
            foreach (var monitor in monitors)
            {
                monitorsWorksheet.Cell(row, 1).Value = monitor.SerialNumber;
                monitorsWorksheet.Cell(row, 2).Value = monitor.Barcode;
                monitorsWorksheet.Cell(row, 3).Value = monitor.Brand;
                monitorsWorksheet.Cell(row, 4).Value = monitor.Model;
                monitorsWorksheet.Cell(row, 5).Value = monitor.Resolution;
                monitorsWorksheet.Cell(row, 6).Value = monitor.Inches;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportEmployeeTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var employees = await context.Employees.ToListAsync();
            var employeesWorksheet = workbook.Worksheets.Add("Στελέχοι");
            employeesWorksheet.Cell(1, 1).Value = "Βαθμός";
            employeesWorksheet.Cell(1, 2).Value = "Όπλο/Σώμα";
            employeesWorksheet.Cell(1, 3).Value = "Όνομα";
            employeesWorksheet.Cell(1, 4).Value = "Επίθετο";
            employeesWorksheet.Cell(1, 5).Value = "Αριθμός Μητρώου";
            employeesWorksheet.Cell(1, 6).Value = "Μονάδα";
            employeesWorksheet.Cell(1, 7).Value = "Γραφείο";
            employeesWorksheet.Cell(1, 8).Value = "Θέση";
            employeesWorksheet.Cell(1, 9).Value = "Όνομα Χρήστη Η/Υ";
            employeesWorksheet.Cell(1, 10).Value = "Όνομα Χρήστη ΣΗΔΑΕΔ";
            employeesWorksheet.Cell(1, 11).Value = "Τηλέφωνο";
            employeesWorksheet.Cell(1, 12).Value = "Κινητό";
            employeesWorksheet.Cell(1, 13).Value = "Email";
            int row = 2;
            foreach (var employee in employees)
            {
                employeesWorksheet.Cell(row, 1).Value = employee.Rank;
                employeesWorksheet.Cell(row, 2).Value = employee.Speciality;
                employeesWorksheet.Cell(row, 3).Value = employee.FirstName;
                employeesWorksheet.Cell(row, 4).Value = employee.LastName;
                employeesWorksheet.Cell(row, 5).Value = employee.AM;
                employeesWorksheet.Cell(row, 6).Value = employee.Unit;
                employeesWorksheet.Cell(row, 7).Value = employee.Office;
                employeesWorksheet.Cell(row, 8).Value = employee.Position;
                employeesWorksheet.Cell(row, 9).Value = employee.PCUsername;
                employeesWorksheet.Cell(row, 10).Value = employee.SHDAEDUsername;
                employeesWorksheet.Cell(row, 11).Value = employee.Phone;
                employeesWorksheet.Cell(row, 12).Value = employee.Mobile;
                employeesWorksheet.Cell(row, 13).Value = employee.Email;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportPCTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var pcs = await context.PCs.ToListAsync();
            var pcsWorksheet = workbook.Worksheets.Add("Ηλεκτονικοί Υπολογιστές");
            pcsWorksheet.Cell(1, 1).Value = "Barcode";
            pcsWorksheet.Cell(1, 2).Value = "Σειριακός Αριθμός";
            pcsWorksheet.Cell(1, 3).Value = "Μάρκα";
            pcsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            pcsWorksheet.Cell(1, 5).Value = "Όνομα Η/Υ";
            pcsWorksheet.Cell(1, 6).Value = "Κωδικός Διαχειριστή";
            pcsWorksheet.Cell(1, 7).Value = "Κωδικός BIOS";
            pcsWorksheet.Cell(1, 8).Value = "Τομέας";
            pcsWorksheet.Cell(1, 9).Value = "Διεύθυνση IP";
            pcsWorksheet.Cell(1, 10).Value = "Εξωτερική IP";
            pcsWorksheet.Cell(1, 11).Value = "Μάσκα Δικτύου";
            pcsWorksheet.Cell(1, 12).Value = "Πύλη";
            pcsWorksheet.Cell(1, 13).Value = "DNS1";
            pcsWorksheet.Cell(1, 14).Value = "DNS2";
            int row = 2;
            foreach (var pc in pcs)
            {
                pcsWorksheet.Cell(row, 1).Value = pc.Barcode;
                pcsWorksheet.Cell(row, 2).Value = pc.SerialNumber;
                pcsWorksheet.Cell(row, 3).Value = pc.Brand;
                pcsWorksheet.Cell(row, 4).Value = pc.Model;
                pcsWorksheet.Cell(row, 5).Value = pc.PCName;
                pcsWorksheet.Cell(row, 6).Value = pc.AdministratorCode;
                pcsWorksheet.Cell(row, 7).Value = pc.BIOSCode;
                pcsWorksheet.Cell(row, 8).Value = pc.Domain;
                pcsWorksheet.Cell(row, 9).Value = pc.IP;
                pcsWorksheet.Cell(row, 10).Value = pc.ExternalIP;
                pcsWorksheet.Cell(row, 11).Value = pc.SubnetMask;
                pcsWorksheet.Cell(row, 12).Value = pc.Gateway;
                pcsWorksheet.Cell(row, 13).Value = pc.DNS1;
                pcsWorksheet.Cell(row, 14).Value = pc.DNS2;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportCPUTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var cpus = await context.CPUs.ToListAsync();
            var cpusWorksheet = workbook.Worksheets.Add("Επεξεργαστές");
            cpusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            cpusWorksheet.Cell(1, 2).Value = "Barcode";
            cpusWorksheet.Cell(1, 3).Value = "Μάρκα";
            cpusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            cpusWorksheet.Cell(1, 5).Value = "Πυρήνες";
            cpusWorksheet.Cell(1, 6).Value = "Νήματα";
            cpusWorksheet.Cell(1, 7).Value = "Χαρακτηριστηκά";
            cpusWorksheet.Cell(1, 8).Value = "Πακέτο";
            cpusWorksheet.Cell(1, 9).Value = "Chipset";
            cpusWorksheet.Cell(1, 10).Value = "Ενσωματωμένα Γραφικά";
            int row = 2;
            foreach (var cpu in cpus)
            {
                cpusWorksheet.Cell(row, 1).Value = cpu.SerialNumber;
                cpusWorksheet.Cell(row, 2).Value = cpu.Barcode;
                cpusWorksheet.Cell(row, 3).Value = cpu.Brand;
                cpusWorksheet.Cell(row, 4).Value = cpu.Model;
                cpusWorksheet.Cell(row, 5).Value = cpu.Cores;
                cpusWorksheet.Cell(row, 6).Value = cpu.Threads;
                cpusWorksheet.Cell(row, 7).Value = cpu.Specification;
                cpusWorksheet.Cell(row, 8).Value = cpu.Package;
                cpusWorksheet.Cell(row, 9).Value = cpu.Chipset;
                cpusWorksheet.Cell(row, 10).Value = cpu.IntegratedGPUModel;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportMOBOTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var mobos = await context.MOBOs.ToListAsync();
            var mobosWorksheet = workbook.Worksheets.Add("Μητρικές Κάρτες");
            mobosWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            mobosWorksheet.Cell(1, 2).Value = "Barcode";
            mobosWorksheet.Cell(1, 3).Value = "Μάρκα";
            mobosWorksheet.Cell(1, 4).Value = "Μοντέλο";
            mobosWorksheet.Cell(1, 5).Value = "Θέσεις Μνημών";
            mobosWorksheet.Cell(1, 6).Value = "Chipset Μάρκα";
            mobosWorksheet.Cell(1, 7).Value = "Chipset Μοντέλο";
            mobosWorksheet.Cell(1, 8).Value = "BIOS Μάρκα";
            mobosWorksheet.Cell(1, 9).Value = "BIOS Έκδοση";
            mobosWorksheet.Cell(1, 10).Value = "GPU Μάρκα";
            mobosWorksheet.Cell(1, 11).Value = "GPU Μοντέλο";
            mobosWorksheet.Cell(1, 12).Value = "Network Μάρκα";
            mobosWorksheet.Cell(1, 13).Value = "Network Μοντέλο";
            int row = 2;
            foreach (var mobo in mobos)
            {
                mobosWorksheet.Cell(row, 1).Value = mobo.SerialNumber;
                mobosWorksheet.Cell(row, 2).Value = mobo.Barcode;
                mobosWorksheet.Cell(row, 3).Value = mobo.Brand;
                mobosWorksheet.Cell(row, 4).Value = mobo.Model;
                mobosWorksheet.Cell(row, 5).Value = mobo.DRAMSlots;
                mobosWorksheet.Cell(row, 6).Value = mobo.ChipsetVendor;
                mobosWorksheet.Cell(row, 7).Value = mobo.ChipsetModel;
                mobosWorksheet.Cell(row, 8).Value = mobo.BIOSBrand;
                mobosWorksheet.Cell(row, 9).Value = mobo.BIOSVersion;
                mobosWorksheet.Cell(row, 10).Value = mobo.OnBoardGPUBrand;
                mobosWorksheet.Cell(row, 11).Value = mobo.OnBoardGPUModel;
                mobosWorksheet.Cell(row, 12).Value = mobo.OnBoardNetworkAdapterBrand;
                mobosWorksheet.Cell(row, 13).Value = mobo.OnBoardNetworkAdapterModel;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportRAMTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var rams = await context.RAMs.ToListAsync();
            var ramsWorksheet = workbook.Worksheets.Add("Μνήμες");
            ramsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            ramsWorksheet.Cell(1, 2).Value = "Barcode";
            ramsWorksheet.Cell(1, 3).Value = "Μάρκα";
            ramsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            ramsWorksheet.Cell(1, 5).Value = "Τύπος";
            ramsWorksheet.Cell(1, 6).Value = "Μέγεθος";
            ramsWorksheet.Cell(1, 7).Value = "Συχνότητα";
            ramsWorksheet.Cell(1, 8).Value = "CAS Latency";
            int row = 2;
            foreach (var ram in rams)
            {
                ramsWorksheet.Cell(row, 1).Value = ram.SerialNumber;
                ramsWorksheet.Cell(row, 2).Value = ram.Barcode;
                ramsWorksheet.Cell(row, 3).Value = ram.Brand;
                ramsWorksheet.Cell(row, 4).Value = ram.Model;
                ramsWorksheet.Cell(row, 5).Value = ram.Type;
                ramsWorksheet.Cell(row, 6).Value = ram.Size;
                ramsWorksheet.Cell(row, 7).Value = ram.Frequency;
                ramsWorksheet.Cell(row, 8).Value = ram.CASLatency;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportGPUTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var gpus = await context.GPUs.ToListAsync();
            var gpusWorksheet = workbook.Worksheets.Add("Κάρτες Γραφικών");
            gpusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            gpusWorksheet.Cell(1, 2).Value = "Barcode";
            gpusWorksheet.Cell(1, 3).Value = "Μάρκα";
            gpusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            gpusWorksheet.Cell(1, 5).Value = "Μνήμη";
            gpusWorksheet.Cell(1, 6).Value = "Driver Έκδοση";
            int row = 2;
            foreach (var gpu in gpus)
            {
                gpusWorksheet.Cell(row, 1).Value = gpu.SerialNumber;
                gpusWorksheet.Cell(row, 2).Value = gpu.Barcode;
                gpusWorksheet.Cell(row, 3).Value = gpu.Brand;
                gpusWorksheet.Cell(row, 4).Value = gpu.Model;
                gpusWorksheet.Cell(row, 5).Value = gpu.Memory;
                gpusWorksheet.Cell(row, 6).Value = gpu.DriverVersion;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportPSUTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var psus = await context.PSUs.ToListAsync();
            var psusWorksheet = workbook.Worksheets.Add("Τροφοδορικά");
            psusWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            psusWorksheet.Cell(1, 2).Value = "Barcode";
            psusWorksheet.Cell(1, 3).Value = "Μάρκα";
            psusWorksheet.Cell(1, 4).Value = "Μοντέλο";
            psusWorksheet.Cell(1, 5).Value = "Ισχύς";
            psusWorksheet.Cell(1, 6).Value = "Πιστοποίηση";
            int row = 2;
            foreach (var psu in psus)
            {
                psusWorksheet.Cell(row, 1).Value = psu.SerialNumber;
                psusWorksheet.Cell(row, 2).Value = psu.Barcode;
                psusWorksheet.Cell(row, 3).Value = psu.Brand;
                psusWorksheet.Cell(row, 4).Value = psu.Model;
                psusWorksheet.Cell(row, 5).Value = psu.Wattage;
                psusWorksheet.Cell(row, 6).Value = psu.Certification;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportStorageTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var storages = await context.Storages.ToListAsync();
            var storagesWorksheet = workbook.Worksheets.Add("Αποθηκευτικά Μέσα");
            storagesWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            storagesWorksheet.Cell(1, 2).Value = "Barcode";
            storagesWorksheet.Cell(1, 3).Value = "Μάρκα";
            storagesWorksheet.Cell(1, 4).Value = "Μοντέλο";
            storagesWorksheet.Cell(1, 5).Value = "Τύπος";
            storagesWorksheet.Cell(1, 6).Value = "Διεπαφή";
            storagesWorksheet.Cell(1, 7).Value = "Ταχύτητα";
            storagesWorksheet.Cell(1, 8).Value = "Χωρητικότητα";
            int row = 2;
            foreach (var storage in storages)
            {
                storagesWorksheet.Cell(row, 1).Value = storage.SerialNumber;
                storagesWorksheet.Cell(row, 2).Value = storage.Barcode;
                storagesWorksheet.Cell(row, 3).Value = storage.Brand;
                storagesWorksheet.Cell(row, 4).Value = storage.Model;
                storagesWorksheet.Cell(row, 5).Value = storage.Type;
                storagesWorksheet.Cell(row, 6).Value = storage.Interface;
                storagesWorksheet.Cell(row, 7).Value = storage.Speed;
                storagesWorksheet.Cell(row, 8).Value = storage.Capacity;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportNetworkCardTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var networkcards = await context.NetworkCards.ToListAsync();
            var networkcardsWorksheet = workbook.Worksheets.Add("Κάρτες Δικτύου");
            networkcardsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            networkcardsWorksheet.Cell(1, 2).Value = "Barcode";
            networkcardsWorksheet.Cell(1, 3).Value = "Μάρκα";
            networkcardsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            int row = 2;
            foreach (var networkcard in networkcards)
            {
                networkcardsWorksheet.Cell(row, 1).Value = networkcard.SerialNumber;
                networkcardsWorksheet.Cell(row, 2).Value = networkcard.Barcode;
                networkcardsWorksheet.Cell(row, 3).Value = networkcard.Brand;
                networkcardsWorksheet.Cell(row, 4).Value = networkcard.Model;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        public async Task<MemoryStream> ExportMonitorTableAsync()
        {
            var workbook = new XLWorkbook();
            var stream = new MemoryStream();
            var monitors = await context.Monitors.ToListAsync();
            var monitorsWorksheet = workbook.Worksheets.Add("Οθόνες");
            monitorsWorksheet.Cell(1, 1).Value = "Σειριακός Αριθμός";
            monitorsWorksheet.Cell(1, 2).Value = "Barcode";
            monitorsWorksheet.Cell(1, 3).Value = "Μάρκα";
            monitorsWorksheet.Cell(1, 4).Value = "Μοντέλο";
            monitorsWorksheet.Cell(1, 5).Value = "Ανάλυση";
            monitorsWorksheet.Cell(1, 6).Value = "Ίντσες";
            int row = 2;
            foreach (var monitor in monitors)
            {
                monitorsWorksheet.Cell(row, 1).Value = monitor.SerialNumber;
                monitorsWorksheet.Cell(row, 2).Value = monitor.Barcode;
                monitorsWorksheet.Cell(row, 3).Value = monitor.Brand;
                monitorsWorksheet.Cell(row, 4).Value = monitor.Model;
                monitorsWorksheet.Cell(row, 5).Value = monitor.Resolution;
                monitorsWorksheet.Cell(row, 6).Value = monitor.Inches;
                row++;
            }
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}