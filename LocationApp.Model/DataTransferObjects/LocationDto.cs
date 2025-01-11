namespace LocationApp.Model.DataTransferObjects
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public decimal? Rating { get; set; }
        public int? RatingsTotal { get; set; }
        public bool IsFavorite { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }
}
