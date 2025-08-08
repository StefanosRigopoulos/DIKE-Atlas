namespace API.Helpers
{
    public class PSUParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Wattage { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}