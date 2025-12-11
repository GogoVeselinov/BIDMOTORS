using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class Car : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        [StringLength(4)]
        public string Year { get; set; } = string.Empty;
        
        [StringLength(17)]
        public string? VIN { get; set; }
        
        [Required]
        [StringLength(20)]
        public string RegistrationNumber { get; set; } = string.Empty;

        // Foreign keys
        [Required]
        public Guid ClientId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;
        
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
