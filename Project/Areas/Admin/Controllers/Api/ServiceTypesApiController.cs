using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.ViewModels.Services;

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

        // GET: api/admin/servicetypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceTypeListItemViewModel>>> GetAll()
        {
            try
            {
                var serviceTypes = await _serviceTypeService.GetAllViewModelsAsync();
                return Ok(serviceTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при зареждане на типовете услуги", error = ex.Message });
            }
        }

        // GET: api/admin/servicetypes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceTypeDetailsViewModel>> GetById(Guid id)
        {
            try
            {
                var serviceType = await _serviceTypeService.GetByIdViewModelAsync(id);
                if (serviceType == null)
                {
                    return NotFound(new { message = "Типът услуга не е намерен" });
                }
                return Ok(serviceType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при зареждане на типа услуга", error = ex.Message });
            }
        }

        // POST: api/admin/servicetypes
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateServiceTypeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Невалидни данни", errors = ModelState });
                }

                var success = await _serviceTypeService.CreateFromViewModelAsync(model);
                if (success)
                {
                    return Ok(new { message = "Типът услуга беше създаден успешно" });
                }

                return BadRequest(new { message = "Неуспешно създаване на тип услуга" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при създаване на типа услуга", error = ex.Message });
            }
        }

        // PUT: api/admin/servicetypes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateServiceTypeViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest(new { message = "ID-то не съвпада" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Невалидни данни", errors = ModelState });
                }

                var success = await _serviceTypeService.UpdateFromViewModelAsync(model);
                if (success)
                {
                    return Ok(new { message = "Типът услуга беше актуализиран успешно" });
                }

                return NotFound(new { message = "Типът услуга не е намерен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при актуализиране на типа услуга", error = ex.Message });
            }
        }

        // DELETE: api/admin/servicetypes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _serviceTypeService.DeleteAsync(id);
                if (success)
                {
                    return Ok(new { message = "Типът услуга беше изтрит успешно" });
                }

                return NotFound(new { message = "Типът услуга не е намерен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при изтриване на типа услуга", error = ex.Message });
            }
        }
    }
}
