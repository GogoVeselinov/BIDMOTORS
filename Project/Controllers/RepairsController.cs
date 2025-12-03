using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class RepairsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
