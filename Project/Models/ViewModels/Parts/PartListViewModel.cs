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
        public string? OEM { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string? Supplier { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInStock => QuantityInStock > 0;
    }
}
