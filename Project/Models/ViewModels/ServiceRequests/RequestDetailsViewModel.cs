using System.Data;

namespace Project.Models.ViewModels.ServiceRequests
{
    public class RequestDetailsViewModel
    {
        public Guid Id { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? Description { get; set; }
        
        // Car details
        public string CarMake { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public string CarYear { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        
        // Client details
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        
        // Repair details (if exists)
        public bool HasRepair { get; set; }
        public Guid? RepairId { get; set; }
        public decimal? RepairPrice { get; set; }
        public string? RepairWorkDescription { get; set; }
        public DateTime? RepairFinishedOn { get; set; }
    }
}
