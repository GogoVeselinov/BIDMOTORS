namespace Project.Models.Entities
{
    public class ServiceRequest : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Cancelled
        public DateTime RequestDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Foreign keys
        public int ClientId { get; set; }
        public int CarId { get; set; }
        public int ServiceTypeId { get; set; }

        // Navigation properties
        public Client Client { get; set; } = null!;
        public Car Car { get; set; } = null!;
        public ServiceType ServiceType { get; set; } = null!;
        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
