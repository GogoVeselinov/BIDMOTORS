using Microsoft.AspNetCore.Mvc;
using Project.Filters;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorization]
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
