namespace Project.Models.ViewModels.Parts
{
    public class PartListViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? PartType { get; set; }
        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }
        public int? CarYear { get; set; }
        public string? OemNumber { get; set; }
        public string? Manufacturer { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Supplier { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInStock => StockQuantity > 0;
    }
}
