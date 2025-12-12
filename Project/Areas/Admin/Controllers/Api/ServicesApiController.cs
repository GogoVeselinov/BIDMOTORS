using Microsoft.AspNetCore.Mvc;
using Project.Areas.Admin.Services;
using Project.Models.Entities;
using Project.Models.Enum;
using Project.Models.ViewModels.Services;

namespace Project.Areas.Admin.Controllers.Api
{
    [ApiController]
    [Route("api/admin/services")]
    public class ServicesApiController : ControllerBase
    {
        private readonly ServiceService _serviceService;

        public ServicesApiController(ServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // === SERVICE ENDPOINTS ===

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _serviceService.GetAllAsync();
            var viewModels = services.Select(s => MapToDetailsViewModel(s)).ToList();
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Услугата не беше намерена" });

            var viewModel = MapToDetailsViewModel(service);
            return Ok(viewModel);
        }

        [HttpGet("servicerequest/{serviceRequestId}")]
        public async Task<IActionResult> GetByServiceRequestId(Guid serviceRequestId)
        {
            var services = await _serviceService.GetByServiceRequestIdAsync(serviceRequestId);
            var viewModels = services.Select(s => MapToDetailsViewModel(s)).ToList();
            return Ok(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = new Service
            {
                Id = Guid.NewGuid(),
                ServiceRequestId = model.ServiceRequestId,
                ServiceTypeId = model.ServiceTypeId,
                Status = ServiceStatus.Pending,
                AssignedEmployeeId = model.AssignedEmployeeId,
                Notes = model.Notes
            };

            var success = await _serviceService.CreateAsync(service);
            if (!success)
                return StatusCode(500, new { message = "Грешка при създаване на услуга" });

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, MapToDetailsViewModel(service));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound(new { message = "Услугата не беше намерена" });

            service.ServiceTypeId = model.ServiceTypeId;
            service.AssignedEmployeeId = model.AssignedEmployeeId;
            service.Notes = model.Notes;
            service.Result = model.Result;

            var success = await _serviceService.UpdateAsync(service);
            if (!success)
                return StatusCode(500, new { message = "Грешка при обновяване на услуга" });

            return Ok(new { message = "Услугата беше обновена успешно" });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusModel model)
        {
            var success = await _serviceService.UpdateStatusAsync(id, model.Status);
            if (!success)
                return NotFound(new { message = "Услугата не беше намерена" });

            return Ok(new { message = "Статусът беше обновен успешно" });
        }

        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignEmployee(Guid id, [FromBody] AssignEmployeeModel model)
        {
            var success = await _serviceService.AssignEmployeeAsync(id, model.EmployeeId);
            if (!success)
                return NotFound(new { message = "Услугата не беше намерена" });

            return Ok(new { message = "Служителят беше назначен успешно" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _serviceService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Услугата не беше намерена" });

            return Ok(new { message = "Услугата беше изтрита успешно" });
        }

        // === TASK ENDPOINTS ===

        [HttpPost("{serviceId}/tasks")]
        public async Task<IActionResult> CreateTask(Guid serviceId, [FromBody] CreateTaskModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = new ServiceTask
            {
                Id = Guid.NewGuid(),
                ServiceId = serviceId,
                Title = model.Title,
                IsCompleted = false,
                Notes = model.Notes
            };

            var success = await _serviceService.CreateTaskAsync(task);
            if (!success)
                return StatusCode(500, new { message = "Грешка при създаване на задача" });

            return Ok(new { id = task.Id, message = "Задачата беше създадена успешно" });
        }

        [HttpPut("tasks/{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskModel model)
        {
            var task = await _serviceService.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound(new { message = "Задачата не беше намерена" });

            task.Title = model.Title;
            task.Notes = model.Notes;

            var success = await _serviceService.UpdateTaskAsync(task);
            if (!success)
                return StatusCode(500, new { message = "Грешка при обновяване на задача" });

            return Ok(new { message = "Задачата беше обновена успешно" });
        }

        [HttpPut("tasks/{taskId}/toggle")]
        public async Task<IActionResult> ToggleTask(Guid taskId, [FromBody] ToggleTaskModel model)
        {
            var success = await _serviceService.ToggleTaskCompletionAsync(taskId, model.EmployeeId);
            if (!success)
                return NotFound(new { message = "Задачата не беше намерена" });

            return Ok(new { message = "Задачата беше обновена успешно" });
        }

        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var success = await _serviceService.DeleteTaskAsync(taskId);
            if (!success)
                return NotFound(new { message = "Задачата не беше намерена" });

            return Ok(new { message = "Задачата беше изтрита успешно" });
        }

        // === PART LINK ENDPOINTS ===

        [HttpPost("{serviceId}/partlinks")]
        public async Task<IActionResult> CreatePartLink(Guid serviceId, [FromBody] CreatePartLinkModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var partLink = new ServicePartLink
            {
                Id = Guid.NewGuid(),
                ServiceId = serviceId,
                Title = model.Title,
                Url = model.Url,
                Supplier = model.Supplier,
                Notes = model.Notes
            };

            var success = await _serviceService.CreatePartLinkAsync(partLink);
            if (!success)
                return StatusCode(500, new { message = "Грешка при създаване на линк" });

            return Ok(new { id = partLink.Id, message = "Линкът беше създаден успешно" });
        }

        [HttpPut("partlinks/{partLinkId}")]
        public async Task<IActionResult> UpdatePartLink(Guid partLinkId, [FromBody] UpdatePartLinkModel model)
        {
            var partLink = await _serviceService.GetPartLinkByIdAsync(partLinkId);
            if (partLink == null)
                return NotFound(new { message = "Линкът не беше намерен" });

            partLink.Title = model.Title;
            partLink.Url = model.Url;
            partLink.Supplier = model.Supplier;
            partLink.Notes = model.Notes;

            var success = await _serviceService.UpdatePartLinkAsync(partLink);
            if (!success)
                return StatusCode(500, new { message = "Грешка при обновяване на линк" });

            return Ok(new { message = "Линкът беше обновен успешно" });
        }

        [HttpDelete("partlinks/{partLinkId}")]
        public async Task<IActionResult> DeletePartLink(Guid partLinkId)
        {
            var success = await _serviceService.DeletePartLinkAsync(partLinkId);
            if (!success)
                return NotFound(new { message = "Линкът не беше намерен" });

            return Ok(new { message = "Линкът беше изтрит успешно" });
        }

        // === HELPER METHODS ===

        private ServiceDetailsViewModel MapToDetailsViewModel(Service service)
        {
            return new ServiceDetailsViewModel
            {
                Id = service.Id,
                ServiceTypeName = service.ServiceType?.Name ?? "N/A",
                Status = service.Status.ToString(),
                AssignedEmployeeName = service.AssignedEmployee?.Name,
                Notes = service.Notes,
                Result = service.Result,
                StartedOn = service.StartedOn,
                CompletedOn = service.CompletedOn,
                CreatedOn = service.CreatedOn,
                ClientName = service.ServiceRequest?.Client?.Name ?? "N/A",
                ClientPhone = service.ServiceRequest?.Client?.Phone ?? "N/A",
                ClientEmail = service.ServiceRequest?.Client?.Email ?? "N/A",
                CarBrand = service.ServiceRequest?.Car?.Brand ?? "N/A",
                CarModel = service.ServiceRequest?.Car?.Model ?? "N/A",
                CarYear = int.TryParse(service.ServiceRequest?.Car?.Year, out var year) ? year : 0,
                VIN = service.ServiceRequest?.Car?.VIN ?? "N/A",
                Tasks = service.Tasks?.Select(t => new ServiceTaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CompletedOn = t.CompletedOn,
                    CompletedByEmployeeName = t.CompletedByEmployee?.Name,
                    Notes = t.Notes
                }).ToList() ?? new List<ServiceTaskViewModel>(),
                PartLinks = service.PartLinks?.Select(pl => new ServicePartLinkViewModel
                {
                    Id = pl.Id,
                    Title = pl.Title,
                    Url = pl.Url,
                    Supplier = pl.Supplier,
                    Notes = pl.Notes
                }).ToList() ?? new List<ServicePartLinkViewModel>()
            };
        }
    }

    // === REQUEST MODELS ===

    public class CreateServiceModel
    {
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? AssignedEmployeeId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateServiceModel
    {
        public Guid ServiceTypeId { get; set; }
        public Guid? AssignedEmployeeId { get; set; }
        public string? Notes { get; set; }
        public string? Result { get; set; }
    }

    public class UpdateStatusModel
    {
        public ServiceStatus Status { get; set; }
    }

    public class AssignEmployeeModel
    {
        public Guid? EmployeeId { get; set; }
    }

    public class CreateTaskModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class UpdateTaskModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class ToggleTaskModel
    {
        public Guid? EmployeeId { get; set; }
    }

    public class CreatePartLinkModel
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Supplier { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePartLinkModel
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Supplier { get; set; }
        public string? Notes { get; set; }
    }
}
