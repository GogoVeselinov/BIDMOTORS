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
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Completed, Archived
        
        public DateTime? FinishedOn { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; } = null!;
        
        [ForeignKey(nameof(RequestId))]
        public ServiceRequest ServiceRequest { get; set; } = null!;
        
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
