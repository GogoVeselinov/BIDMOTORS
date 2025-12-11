using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.Entities;

namespace Project.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/servicetypes")]
    [ApiController]
    public class ServiceTypesApiController : ControllerBase
    {
        private readonly ServiceTypeService _serviceTypeService;

        public ServiceTypesApiController(ServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceTypes = await _serviceTypeService.GetAllAsync();
            return Ok(serviceTypes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var serviceType = await _serviceTypeService.GetByIdAsync(id);
            if (serviceType == null)
                return NotFound();
            return Ok(serviceType);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceType serviceType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _serviceTypeService.CreateAsync(serviceType);
            if (success)
                return Ok(new { message = "Типът услуга беше създаден успешно!" });

            return BadRequest(new { message = "Грешка при създаване" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ServiceType serviceType)
        {
            if (id != serviceType.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _serviceTypeService.UpdateAsync(serviceType);
            if (success)
                return Ok(new { message = "Типът услуга беше актуализиран успешно!" });

            return BadRequest(new { message = "Грешка при актуализиране" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _serviceTypeService.DeleteAsync(id);
            if (success)
                return Ok(new { message = "Типът услуга беше изтрит успешно!" });

            return BadRequest(new { message = "Грешка при изтриване" });
        }
    }
}
