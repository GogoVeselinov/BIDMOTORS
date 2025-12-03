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
        [Range(1900, 2100)]
        public int Year { get; set; }
        
        [StringLength(17)]
        public string? VIN { get; set; }

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
