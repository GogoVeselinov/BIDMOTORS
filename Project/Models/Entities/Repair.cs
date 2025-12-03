namespace Project.Models.Entities
{
    public class Repair : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public decimal LaborCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty; // InProgress, Completed
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Foreign keys
        public int ServiceRequestId { get; set; }
        public int CarId { get; set; }
        public int EmployeeId { get; set; }

        // Navigation properties
        public ServiceRequest ServiceRequest { get; set; } = null!;
        public Car Car { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();
    }
}
