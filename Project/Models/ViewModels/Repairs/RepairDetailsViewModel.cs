namespace Project.Models.ViewModels.Repairs
{
    public class RepairDetailsViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal LaborCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CarInfo { get; set; } = string.Empty;
        public string MechanicName { get; set; } = string.Empty;
    }
}
