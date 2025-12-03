using System.ComponentModel.DataAnnotations;

namespace Project.Models.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? PasswordHash { get; set; }

        // Navigation properties
        public ICollection<Car> Cars { get; set; } = new List<Car>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
