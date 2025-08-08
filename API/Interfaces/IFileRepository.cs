using OfficeOpenXml;
using API.Helpers;

namespace API.Interfaces
{
    public interface IFileRepository
    {
        Task<bool> ImportDBAsync(DataModel data);
        Task<bool> ImportEmployeeTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportPCTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportCPUTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportMOBOTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportRAMTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportGPUTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportPSUTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportStorageTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportNetworkCardTableAsync(ExcelWorksheet worksheet, int rows);
        Task<bool> ImportMonitorTableAsync(ExcelWorksheet worksheet, int rows);
        Task<DataModel> ExportDBAsync();
        Task<MemoryStream> ExportDatabaseAsync();
        Task<MemoryStream> ExportEmployeeTableAsync();
        Task<MemoryStream> ExportPCTableAsync();
        Task<MemoryStream> ExportCPUTableAsync();
        Task<MemoryStream> ExportMOBOTableAsync();
        Task<MemoryStream> ExportRAMTableAsync();
        Task<MemoryStream> ExportGPUTableAsync();
        Task<MemoryStream> ExportPSUTableAsync();
        Task<MemoryStream> ExportStorageTableAsync();
        Task<MemoryStream> ExportNetworkCardTableAsync();
        Task<MemoryStream> ExportMonitorTableAsync();
    }
}