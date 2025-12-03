namespace Project.Models.ViewModels.ServiceRequests
{
    public class ServiceRequestFormViewModel
    {
        public int CarId { get; set; }
        public int ServiceTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
