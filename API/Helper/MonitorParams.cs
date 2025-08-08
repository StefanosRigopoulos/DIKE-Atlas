namespace API.Helpers
{
    public class MonitorParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Resolution { get; set; }
        public string? Inches { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}