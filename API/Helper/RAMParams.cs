namespace API.Helpers
{
    public class RAMParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Type { get; set; }
        public string? Size { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}