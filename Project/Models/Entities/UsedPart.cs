namespace Project.Models.Entities
{
    public class UsedPart : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Foreign keys
        public int RepairId { get; set; }
        public int PartId { get; set; }

        // Navigation properties
        public Repair Repair { get; set; } = null!;
        public Part Part { get; set; } = null!;
    }
}
