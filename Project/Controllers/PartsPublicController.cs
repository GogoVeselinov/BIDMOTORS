using Microsoft.AspNetCore.Mvc;
using Project.Models.ViewModels.Parts;
using Project.Services;

namespace Project.Controllers
{
    public class PartsPublicController : Controller
    {
        private readonly PartService _partService;

        public PartsPublicController(PartService partService)
        {
            _partService = partService;
        }

        public async Task<IActionResult> Index(PartFilterModel filters)
        {
            filters ??= new PartFilterModel();
            
            var parts = await _partService.GetFilteredPartsAsync(filters);
            
            var model = new PartsPageViewModel
            {
                Filters = filters,
                Parts = parts
            };

            return View("~/Views/Parts/Index.cshtml", model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var part = await _partService.GetPartByIdAsync(id);
            
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }
    }
}
