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
        private readonly InvoiceService _invoiceService;

        public RepairsController(ApplicationDbContext context, RepairService repairService, InvoiceService invoiceService)
        {
            _context = context;
            _repairService = repairService;
            _invoiceService = invoiceService;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateManagerNotes(Guid id, string? managerNotes)
        {
            var repair = await _context.Repairs.FindAsync(id);

            if (repair == null)
            {
                TempData["ErrorMessage"] = "Ремонтът не е намерен";
                return RedirectToAction(nameof(Index));
            }

            if (repair.Status != "Active")
            {
                TempData["ErrorMessage"] = "Не може да редактирате описанието на завършен ремонт";
                return RedirectToAction(nameof(Details), new { id });
            }

            repair.ManagerNotes = managerNotes;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Описанието е запазено успешно";
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
        public async Task<IActionResult> RemovePart([FromBody] RemovePartModel model)
        {
            var success = await _repairService.RemovePartFromRepairAsync(model.UsedPartId);

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Частта е премахната успешно"
                });
            }

            return Json(new { success = false, message = "Грешка при премахване на частта" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDuplicates(Guid id)
        {
            var success = await _repairService.RemoveDuplicatePartsAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Дублираните части са премахнати успешно";
            }
            else
            {
                TempData["ErrorMessage"] = "Грешка при премахване на дубликати";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecalculateCosts(Guid id)
        {
            var success = await _repairService.RecalculateRepairCostsAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Цените са преизчислени успешно";
            }
            else
            {
                TempData["ErrorMessage"] = "Грешка при преизчисляване на цените";
            }

            return RedirectToAction(nameof(Details), new { id });
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
            try
            {
                var pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(id);
                
                var repair = await _repairService.GetRepairDetailsAsync(id);
                var fileName = $"Invoice_{repair?.InvoiceNumber ?? id.ToString().Substring(0, 8)}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Грешка при генериране на фактура: " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }

    public class AddPartModel
    {
        public Guid RepairId { get; set; }
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemovePartModel
    {
        public Guid UsedPartId { get; set; }
    }
}
