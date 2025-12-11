using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.Entities;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PartsController : Controller
    {
        private readonly AdminPartService _adminPartService;

        public PartsController(AdminPartService adminPartService)
        {
            _adminPartService = adminPartService;
        }

        // GET: Admin/Parts/Index
        public async Task<IActionResult> Index(string? search)
        {
            List<Part> parts;
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                parts = await _adminPartService.SearchAsync(search);
                ViewData["SearchTerm"] = search;
            }
            else
            {
                parts = await _adminPartService.GetAllAsync();
            }

            return View(parts);
        }

        // GET: Admin/Parts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Part part)
        {
            if (!ModelState.IsValid)
            {
                return View(part);
            }

            var success = await _adminPartService.CreateAsync(part);
            if (success)
            {
                TempData["SuccessMessage"] = "Частта беше създадена успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при създаването на частта.";
            return View(part);
        }

        // GET: Admin/Parts/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var part = await _adminPartService.GetByIdAsync(id);
            if (part == null)
            {
                return NotFound();
            }
            return View(part);
        }

        // POST: Admin/Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Part part)
        {
            if (id != part.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(part);
            }

            var success = await _adminPartService.UpdateAsync(part);
            if (success)
            {
                TempData["SuccessMessage"] = "Частта беше актуализирана успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при актуализирането на частта.";
            return View(part);
        }

        // POST: Admin/Parts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _adminPartService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Частта беше изтрита успешно!";
            }
            else
            {
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на частта.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
