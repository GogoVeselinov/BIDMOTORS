using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Filters;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorization]
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/Users/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/UpdateInline (API endpoint for inline editing)
        [HttpPost]
        public async Task<IActionResult> UpdateInline([FromBody] UserInlineUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Невалидни данни" });
            }

            var user = await _userService.GetByIdAsync(model.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "Потребителят не е намерен" });
            }

            var oldRole = user.Role;
            var newRole = model.Role;

            // Проверка дали ролята се променя към Admin или Manager
            if ((newRole == "Admin" || newRole == "Manager") && oldRole != "Admin" && oldRole != "Manager")
            {
                // Преместване от Clients към Employees
                var (success, message) = await _userService.ConvertClientToEmployeeAsync(model.Id, newRole);
                return Json(new { success, message, redirectToEmployees = success });
            }
            // Проверка дали ролята се променя от Admin/Manager към User
            else if ((oldRole == "Admin" || oldRole == "Manager") && newRole == "User")
            {
                return Json(new { success = false, message = "Не може да промените роля на служител към потребител от тази страница. Използвайте секцията Служители." });
            }
            else
            {
                // Обикновено обновяване без преместване
                user.Name = model.Name;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Role = model.Role;

                var success = await _userService.UpdateAsync(user);
                
                if (success)
                {
                    return Json(new { success = true, message = "Данните са обновени успешно" });
                }

                return Json(new { success = false, message = "Грешка при обновяване на данните" });
            }
        }
    }

    // Model for inline update
    public class UserInlineUpdateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
