namespace API.Helpers
{
    public class CPUParams : PaginationParams
    {
        public string? Barcode { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Cores { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}