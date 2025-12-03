namespace Project.Models.Entities
{
    public class Part : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }

        // Navigation properties
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
