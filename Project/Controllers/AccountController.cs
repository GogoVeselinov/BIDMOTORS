using Microsoft.AspNetCore.Mvc;
using Project.Models.ViewModels.Account;
using Project.Services.Interfaces;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
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
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
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
            // Ако не е логнат, пренасочи към Login
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
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
