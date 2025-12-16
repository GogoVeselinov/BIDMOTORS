using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class Repair : BaseEntity
    {
        [Required]
        public Guid ClientId { get; set; }
        
        [Required]
        public Guid CarId { get; set; }
        
        [Required]
        public Guid RequestId { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string WorkDescription { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal LaborHours { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal LaborCost { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal PartsCost { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal TotalCost { get; set; } = 0;
        
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }
        
        public DateTime? InvoiceGeneratedOn { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Completed, Archived
        
        public DateTime? FinishedOn { get; set; }

        // Navigation properties
        public Client Client { get; set; } = null!;
        public Car Car { get; set; } = null!;
        public ServiceRequest ServiceRequest { get; set; } = null!;
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
