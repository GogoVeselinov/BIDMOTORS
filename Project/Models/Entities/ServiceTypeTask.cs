using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class ServiceTypeTask : BaseEntity
    {
        [Required]
        public Guid ServiceTypeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public int OrderIndex { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Navigation properties
        [ForeignKey(nameof(ServiceTypeId))]
        public ServiceType ServiceType { get; set; } = null!;
    }
}
