using Microsoft.AspNetCore.Mvc;
using Project.Models.ViewModels.Account;
using Project.Services.Interfaces;
using Project.Data;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _db;

        public AccountController(IAuthService authService, ApplicationDbContext db)
        {
            _authService = authService;
            _db = db;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Ако вече е логнат, пренасочи към Home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Опитваме се да влезем като клиент
            var clientResult = await _authService.LoginClientAsync(model);
            
            if (clientResult.Success && clientResult.Client != null)
            {
                // Запазваме данни в сесията
                HttpContext.Session.SetString("UserId", clientResult.Client.Id.ToString());
                HttpContext.Session.SetString("UserEmail", clientResult.Client.Email);
                HttpContext.Session.SetString("UserName", clientResult.Client.Name);
                HttpContext.Session.SetString("UserType", "Client");

                TempData["SuccessMessage"] = "Успешно влизане!";
                return RedirectToAction("Index", "Home");
            }

            // Опитваме се да влезем като служител
            var employeeResult = await _authService.LoginEmployeeAsync(model);
            
            if (employeeResult.Success && employeeResult.Employee != null)
            {
                // Запазваме данни в сесията
                HttpContext.Session.SetString("UserId", employeeResult.Employee.Id.ToString());
                HttpContext.Session.SetString("UserEmail", employeeResult.Employee.Email);
                HttpContext.Session.SetString("UserName", employeeResult.Employee.Name);
                HttpContext.Session.SetString("UserType", "Employee");
                HttpContext.Session.SetString("UserRole", employeeResult.Employee.Role);

                TempData["SuccessMessage"] = "Успешно влизане!";
                
                // Ако е Admin, препращаме към Admin panel
                if (employeeResult.Employee.Role == "Admin")
                {
                    return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home");
            }

            // И двете опита са неуспешни
            ModelState.AddModelError(string.Empty, "Невалиден имейл или парола");
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Ако вече е логнат, пренасочи към Home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterClientAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            // Успешна регистрация - влизаме автоматично
            if (result.Client != null)
            {
                HttpContext.Session.SetString("UserId", result.Client.Id.ToString());
                HttpContext.Session.SetString("UserEmail", result.Client.Email);
                HttpContext.Session.SetString("UserName", result.Client.Name);
                HttpContext.Session.SetString("UserType", "Client");

                TempData["SuccessMessage"] = "Регистрацията е успешна! Добре дошли!";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Profile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            ProfileViewModel model;

            if (userType == "Client")
            {
                var client = _db.Clients.FirstOrDefault(c => c.Id == userId);
                if (client == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                model = new ProfileViewModel
                {
                    FullName = client.Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    CreatedOn = client.CreatedOn,
                    Role = "Клиент",
                    LastLogin = "Днес"
                };
            }
            else // Employee
            {
                var employee = _db.Employees.FirstOrDefault(e => e.Id == userId);
                if (employee == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                model = new ProfileViewModel
                {
                    FullName = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    CreatedOn = employee.CreatedOn,
                    Role = employee.Role,
                    LastLogin = "Днес"
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            if (userType == "Client")
            {
                var client = _db.Clients.FirstOrDefault(c => c.Id == userId);
                if (client != null)
                {
                    client.Name = model.FullName;
                    client.Email = model.Email;
                    client.Phone = model.Phone ?? string.Empty;

                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    }

                    await _db.SaveChangesAsync();
                }
            }
            else // Employee
            {
                var employee = _db.Employees.FirstOrDefault(e => e.Id == userId);
                if (employee != null)
                {
                    employee.Name = model.FullName;
                    employee.Email = model.Email;
                    employee.Phone = model.Phone ?? string.Empty;

                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    }

                    await _db.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Профилът е обновен успешно!";
            return RedirectToAction("Profile");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Излязохте успешно от акаунта си";
            return RedirectToAction("Index", "Home");
        }
    }
}
