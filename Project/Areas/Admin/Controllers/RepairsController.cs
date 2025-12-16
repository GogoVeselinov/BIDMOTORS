using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Services;
using Project.Filters;
using Microsoft.EntityFrameworkCore;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorization]
    public class RepairsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RepairService _repairService;

        public RepairsController(ApplicationDbContext context, RepairService repairService)
        {
            _context = context;
            _repairService = repairService;
        }

        public async Task<IActionResult> Index()
        {
            var repairs = await _context.Repairs
                .Include(r => r.Client)
                .Include(r => r.Car)
                .Include(r => r.ServiceRequest)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            return View(repairs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var repair = await _repairService.GetRepairDetailsAsync(id);

            if (repair == null)
            {
                return NotFound();
            }

            // Зареждане на настройките за показване в детайлите
            ViewBag.Settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);
            ViewBag.AvailableParts = await _context.Parts
                .Where(p => p.IsActive && p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(repair);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLaborHours(Guid id, decimal hours)
        {
            var success = await _repairService.UpdateLaborHoursAsync(id, hours);

            if (success)
            {
                TempData["SuccessMessage"] = "Работните часове са обновени успешно";
            }
            else
            {
                TempData["ErrorMessage"] = "Грешка при обновяване на работните часове";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> AddPart([FromBody] AddPartModel model)
        {
            var success = await _repairService.AddPartToRepairAsync(model.RepairId, model.PartId, model.Quantity);

            if (success)
            {
                var repair = await _repairService.GetRepairDetailsAsync(model.RepairId);
                return Json(new
                {
                    success = true,
                    message = "Частта е добавена успешно",
                    partsCost = repair?.PartsCost,
                    totalCost = repair?.TotalCost
                });
            }

            return Json(new { success = false, message = "Грешка при добавяне на частта" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRepair(Guid id)
        {
            var success = await _repairService.CompleteRepairAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Ремонтът е завършен успешно";
            }
            else
            {
                TempData["ErrorMessage"] = "Грешка при завършване на ремонта";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateInvoice(Guid id)
        {
            var repair = await _repairService.GetRepairDetailsAsync(id);

            if (repair == null || repair.Status != "Completed")
            {
                TempData["ErrorMessage"] = "Ремонтът не е завършен или не съществува";
                return RedirectToAction(nameof(Details), new { id });
            }

            var settings = await _context.PriceSettings.FirstOrDefaultAsync(s => s.IsActive);

            // TODO: Implement PDF generation using QuestPDF
            // За момента връщаме view с данните за фактурата
            ViewBag.Settings = settings;
            return View("Invoice", repair);
        }
    }

    public class AddPartModel
    {
        public Guid RepairId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
    }
}
