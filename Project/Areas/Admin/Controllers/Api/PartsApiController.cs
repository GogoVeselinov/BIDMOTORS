using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;

namespace Project.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/parts")]
    [ApiController]
    public class PartsApiController : ControllerBase
    {
        private readonly AdminPartService _partService;

        public PartsApiController(AdminPartService partService)
        {
            _partService = partService;
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
