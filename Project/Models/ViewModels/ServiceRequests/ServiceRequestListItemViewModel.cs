namespace Project.Models.ViewModels.ServiceRequests
{
    public class ServiceRequestListItemViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string CarInfo { get; set; } = string.Empty;
    }
}
