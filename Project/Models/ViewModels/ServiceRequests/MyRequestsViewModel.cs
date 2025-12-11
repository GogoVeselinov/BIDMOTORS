namespace Project.Models.ViewModels.ServiceRequests
{
    public class MyRequestsViewModel
    {
        public Guid Id { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string CarInfo { get; set; } = string.Empty; // Марка, модел, регистрационен номер
        public string? Description { get; set; }
    }
}
