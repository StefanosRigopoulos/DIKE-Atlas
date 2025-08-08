namespace API.DTOs
{
    public class SearchResultDTO
    {
        public string Type { get; set; } = "";
        public int ID { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}