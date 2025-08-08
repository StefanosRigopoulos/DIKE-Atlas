namespace API.Helpers
{
    public class GPUParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Memory { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}