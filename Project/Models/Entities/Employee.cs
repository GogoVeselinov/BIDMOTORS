using System.ComponentModel.DataAnnotations;

namespace Project.Models.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty; // Admin, Manager

        // Navigation properties
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
