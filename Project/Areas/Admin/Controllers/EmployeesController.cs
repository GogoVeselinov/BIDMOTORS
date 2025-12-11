using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.Entities;
using Project.Models.ViewModels.Admin;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Admin/Employees/Index
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees);
        }

        // GET: Admin/Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Role = model.Role
            };

            var success = await _employeeService.CreateAsync(employee, model.Password);
            if (success)
            {
                TempData["SuccessMessage"] = "Служителят беше създаден успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при създаването на служителя.";
            return View(model);
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeFormViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                Role = employee.Role
            };

            return View(model);
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EmployeeFormViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Phone = model.Phone;
            employee.Role = model.Role;

            var success = await _employeeService.UpdateAsync(employee, model.Password);
            if (success)
            {
                TempData["SuccessMessage"] = "Служителят беше актуализиран успешно!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Възникна грешка при актуализирането на служителя.";
            return View(model);
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _employeeService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Служителят беше изтрит успешно!";
            }
            else
            {
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на служителя.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
