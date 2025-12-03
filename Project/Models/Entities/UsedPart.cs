using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class UsedPart : BaseEntity
    {
        [Required]
        public Guid RepairId { get; set; }
        
        [Required]
        public Guid PartId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityUsed { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal UnitPriceAtMoment { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RepairId))]
        public Repair Repair { get; set; } = null!;
        
        [ForeignKey(nameof(PartId))]
        public Part Part { get; set; } = null!;
    }
}
