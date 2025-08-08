namespace API.Helpers
{
    public class NetworkCardParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}