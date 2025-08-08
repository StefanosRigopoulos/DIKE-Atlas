namespace API.Helpers
{
    public class MOBOParams : PaginationParams
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? DRAMSlots { get; set; }
        public string? ChipsetModel { get; set; }
        public string OrderBy { get; set; } = "brand";
    }
}