using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Filters;
using Project.Models.Entities;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorization]
    public class SettingsController : Controller
    {
        private readonly SettingsService _settingsService;

        public SettingsController(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetActiveSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PriceSettings model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Невалидни данни";
                return View("Index", model);
            }

            var success = await _settingsService.UpdateSettingsAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Настройките са обновени успешно";
            }
            else
            {
                TempData["ErrorMessage"] = "Грешка при запазване на настройките";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
