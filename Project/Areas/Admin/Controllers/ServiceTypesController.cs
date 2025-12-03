using Microsoft.AspNetCore.Mvc;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceTypesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
