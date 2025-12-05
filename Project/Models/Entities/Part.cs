using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class Part : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; }
        
        [StringLength(100)]
        public string? PartType { get; set; }
        
        [StringLength(100)]
        public string? CarBrand { get; set; }
        
        [StringLength(100)]
        public string? CarModel { get; set; }
        
        [Range(1900, 2100)]
        public int? CarYear { get; set; }
        
        [StringLength(100)]
        public string? OEM { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }
        
        [StringLength(200)]
        public string? Supplier { get; set; }
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        // Navigation properties
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
