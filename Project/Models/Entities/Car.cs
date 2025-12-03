namespace Project.Models.Entities
{
    public class Car : BaseEntity
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;

        // Foreign keys
        public int ClientId { get; set; }

        // Navigation properties
        public Client Client { get; set; } = null!;
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
