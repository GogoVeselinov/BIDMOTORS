using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class PartsPublicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
