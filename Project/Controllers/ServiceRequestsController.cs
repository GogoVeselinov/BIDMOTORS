using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class ServiceRequestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
