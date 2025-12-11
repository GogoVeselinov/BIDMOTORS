using Microsoft.AspNetCore.Mvc;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceRequestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            ViewBag.RequestId = id;
            return View();
        }
    }
}
