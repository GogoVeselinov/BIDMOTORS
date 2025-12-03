namespace Project.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalClients { get; set; }
        public int TotalServiceRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ActiveRepairs { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
