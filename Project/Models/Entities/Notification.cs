using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class Notification : BaseEntity
    {
        // User can be either Client or Employee
        public Guid? ClientId { get; set; }
        public Guid? EmployeeId { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public bool IsRead { get; set; } = false;

        [StringLength(50)]
        public string? Type { get; set; } // "ServiceRequest", "Repair", "Part", etc.

        public Guid? RelatedEntityId { get; set; } // ID на свързаната заявка/ремонт

        // Navigation properties
        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
        
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }
    }
}
