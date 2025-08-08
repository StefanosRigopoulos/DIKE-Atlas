namespace API.Helpers
{
    public class StorageParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Type { get; set; }
        public string? Interface { get; set; }
        public string? Capacity { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}