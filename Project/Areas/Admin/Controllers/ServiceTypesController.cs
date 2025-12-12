using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceTypesController : Controller
    {
        private readonly ServiceTypeService _service;

        public ServiceTypesController(ServiceTypeService service)
        {
            _service = service;
        }

        // GET: Admin/ServiceTypes/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/ServiceTypes/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _service.GetByIdViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: Admin/ServiceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Admin/ServiceTypes/Edit/5
        public IActionResult Edit(Guid id)
        {
            ViewData["ServiceTypeId"] = id;
            return View();
        }
    }
}
