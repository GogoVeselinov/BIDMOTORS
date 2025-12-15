using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels.Services
{
    public class UpdateServiceTypeViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    public class ServiceTypeDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<ServiceTypeTaskViewModel> Tasks { get; set; } = new();
        public List<ServiceTypePartViewModel> Parts { get; set; } = new();
    }

    public class ServiceTypeTaskViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int OrderIndex { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ServiceTypePartViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Supplier { get; set; }
        public string? Notes { get; set; }
    }
}
