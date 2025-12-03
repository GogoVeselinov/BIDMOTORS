namespace Project.Models.ViewModels.Repairs
{
    public class RepairListItemViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime StartDate { get; set; }
        public string CarInfo { get; set; } = string.Empty;
    }
}
