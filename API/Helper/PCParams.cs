namespace API.Helpers
{
    public class PCParams : PaginationParams
    {
        public string? Barcode { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Domain { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}