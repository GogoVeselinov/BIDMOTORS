using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.Entities;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceTypesController : Controller
    {
        private readonly ServiceTypeService _serviceTypeService;

        public ServiceTypesController(ServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        // GET: Admin/ServiceTypes/Index
        public async Task<IActionResult> Index()
        {
            var serviceTypes = await _serviceTypeService.GetAllAsync();
            return View(serviceTypes);
        }

        // GET: Admin/ServiceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ServiceTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceType serviceType)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceType);
            }

            var success = await _serviceTypeService.CreateAsync(serviceType);
            if (success)
            {
                TempData["SuccessMessage"] = "Типът услуга беше създаден успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при създаването на типа услуга.";
            return View(serviceType);
        }

        // GET: Admin/ServiceTypes/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var serviceType = await _serviceTypeService.GetByIdAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        // POST: Admin/ServiceTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ServiceType serviceType)
        {
            if (id != serviceType.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(serviceType);
            }

            var success = await _serviceTypeService.UpdateAsync(serviceType);
            if (success)
            {
                TempData["SuccessMessage"] = "Типът услуга беше актуализиран успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при актуализирането на типа услуга.";
            return View(serviceType);
        }

        // POST: Admin/ServiceTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _serviceTypeService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Типът услуга беше изтрит успешно!";
            }
            else
            {
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на типа услуга.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
