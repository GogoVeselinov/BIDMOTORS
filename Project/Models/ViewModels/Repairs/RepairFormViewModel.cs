namespace Project.Models.ViewModels.Repairs
{
    public class RepairFormViewModel
    {
        public int ServiceRequestId { get; set; }
        public int CarId { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal LaborCost { get; set; }
    }
}
