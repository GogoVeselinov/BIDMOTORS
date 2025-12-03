using System.ComponentModel.DataAnnotations;

namespace Project.Models.Entities
{
    public class ServiceType : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
