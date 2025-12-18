using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Filters;

namespace Project.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/parts")]
    [ApiController]
    [AdminAuthorization]
    public class PartsApiController : ControllerBase
    {
        private readonly AdminPartService _partService;

        public PartsApiController(AdminPartService partService)
        {
            _partService = partService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? name,
            [FromQuery] string? category,
            [FromQuery] string? oem,
            [FromQuery] string? manufacturer,
            [FromQuery] string? brand,
            [FromQuery] string? model,
            [FromQuery] string? isActive,
            [FromQuery] string? stock)
        {
            var parts = await _partService.GetFilteredAsync(
                name, 
                category, 
                oem, 
                manufacturer, 
                brand, 
                model, 
                isActive, 
                stock);
            
            return Ok(parts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var part = await _partService.GetByIdAsync(id);
            if (part == null)
                return NotFound();

            return Ok(part);
        }
    }
}
