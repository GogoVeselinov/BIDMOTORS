using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Controllers
{
    public class RepairsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepairsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var repairs = await _context.Repairs
                .Include(r => r.Client)
                .Include(r => r.Car)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            return View(repairs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var repair = await _context.Repairs
                .Include(r => r.Client)
                .Include(r => r.Car)
                .Include(r => r.ServiceRequest)
                .Include(r => r.UsedParts)
                    .ThenInclude(up => up.Part)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (repair == null)
            {
                return NotFound();
            }

            return View(repair);
        }
    }
}
