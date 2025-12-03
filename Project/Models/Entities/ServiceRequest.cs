using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class ServiceRequest : BaseEntity
    {
        [Required]
        public Guid ClientId { get; set; }
        
        [Required]
        public Guid CarId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ServiceType { get; set; } = string.Empty; // Repair, Diagnostic, Safety
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Comment { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, InReview, InProgress, WaitingForParts, Completed, Canceled
        
        [StringLength(2000)]
        public string? InternalNotes { get; set; }
        
        public Guid? LinkedRepairId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; } = null!;
        
        [ForeignKey(nameof(LinkedRepairId))]
        public Repair? LinkedRepair { get; set; }
    }
}
