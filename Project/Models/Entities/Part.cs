using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class Part : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }
        
        [StringLength(200)]
        public string? Supplier { get; set; }

        // Navigation properties
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
