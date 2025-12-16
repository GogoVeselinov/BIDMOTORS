using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.ViewModels.Admin;
using Project.Filters;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorization]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalClients = await _context.Clients.CountAsync(),
                TotalServiceRequests = await _context.ServiceRequests.CountAsync(),
                PendingRequests = await _context.ServiceRequests.CountAsync(sr => sr.Status == "Pending"),
                ActiveRepairs = await _context.Repairs.CountAsync(r => r.Status == "Active"),
                TotalRevenue = await _context.Repairs.Where(r => r.Status == "Completed").SumAsync(r => r.Price)
            };

            // Вземаме последните 5 заявки за таблото
            ViewBag.RecentRequests = await _context.ServiceRequests
                .Include(sr => sr.Client)
                .Include(sr => sr.Car)
                .OrderByDescending(sr => sr.CreatedOn)
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }

        public IActionResult Settings()
        {
            return View();
        }
        
    }
}
