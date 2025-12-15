using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models.Entities
{
    public class ServiceTypePart : BaseEntity
    {
        [Required]
        public Guid ServiceTypeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Supplier { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ServiceTypeId))]
        public ServiceType ServiceType { get; set; } = null!;
    }
}
