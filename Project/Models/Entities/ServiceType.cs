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

        // Navigation properties
        public ICollection<ServiceTypeTask> Tasks { get; set; } = new List<ServiceTypeTask>();
        public ICollection<ServiceTypePart> Parts { get; set; } = new List<ServiceTypePart>();
    }
}
