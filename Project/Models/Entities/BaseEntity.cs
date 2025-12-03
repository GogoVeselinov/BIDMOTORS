using System.ComponentModel.DataAnnotations;

namespace Project.Models.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
