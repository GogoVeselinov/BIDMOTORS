using System;
using System.Collections.Generic;

namespace Project.Models.ViewModels.Services
{
    public class ServiceDetailsViewModel
    {
        public Guid Id { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedEmployeeName { get; set; }
        public string? Notes { get; set; }
        public string? Result { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        // Client Info
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;

        // Car Info
        public string CarBrand { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public int CarYear { get; set; }
        public string VIN { get; set; } = string.Empty;

        // Tasks and Parts
        public List<ServiceTaskViewModel> Tasks { get; set; } = new();
        public List<ServicePartLinkViewModel> PartLinks { get; set; } = new();
    }

    public class ServiceTaskViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string? CompletedByEmployeeName { get; set; }
        public string? Notes { get; set; }
    }

    public class ServicePartLinkViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Supplier { get; set; }
        public string? Notes { get; set; }
    }
}
