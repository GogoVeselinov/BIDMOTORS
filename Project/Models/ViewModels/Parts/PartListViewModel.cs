namespace Project.Models.ViewModels.Parts
{
    public class PartListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsInStock => QuantityInStock > 0;
    }
}
