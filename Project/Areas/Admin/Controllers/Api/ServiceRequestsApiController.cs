using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;
using Project.Services;
using Project.Filters;

namespace Project.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/servicerequests")]
    [ApiController]
    [AdminAuthorization]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? requestId,
            [FromQuery] string? client,
            [FromQuery] string? phone,
            [FromQuery] string? email,
            [FromQuery] string? brand,
            [FromQuery] string? model,
            [FromQuery] string? registration,
            [FromQuery] string? serviceType,
            [FromQuery] string? status,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo)
        {
            var query = _context.ServiceRequests
                .Include(sr => sr.Client)
                .Include(sr => sr.Car)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(requestId))
            {
                // Search by first 8 characters of ID
                query = query.Where(sr => sr.Id.ToString().StartsWith(requestId.ToLower()));
            }

            if (!string.IsNullOrEmpty(client))
                query = query.Where(sr => sr.Client.Name.Contains(client));

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(sr => sr.Client.Phone.Contains(phone));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(sr => sr.Client.Email != null && sr.Client.Email.Contains(email));

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(sr => sr.Car.Brand.Contains(brand));

            if (!string.IsNullOrEmpty(model))
                query = query.Where(sr => sr.Car.Model.Contains(model));

            if (!string.IsNullOrEmpty(registration))
                query = query.Where(sr => sr.Car.RegistrationNumber.Contains(registration));

            if (!string.IsNullOrEmpty(serviceType))
                query = query.Where(sr => sr.ServiceType == serviceType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(sr => sr.Status == status);

            if (dateFrom.HasValue)
                query = query.Where(sr => sr.CreatedOn >= dateFrom.Value);

            if (dateTo.HasValue)
            {
                var dateToEnd = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(sr => sr.CreatedOn <= dateToEnd);
            }

            var requests = await query
                .OrderByDescending(sr => sr.CreatedOn)
                .Select(sr => new
                {
                    sr.Id,
                    sr.ServiceType,
                    sr.Description,
                    sr.Status,
                    sr.CreatedOn,
                    ClientName = sr.Client.Name,
                    ClientPhone = sr.Client.Phone,
                    ClientEmail = sr.Client.Email,
                    CarInfo = sr.Car.Brand + " " + sr.Car.Model + " (" + sr.Car.Year + ")",
                    sr.Car.RegistrationNumber
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var request = await _context.ServiceRequests
                .Include(sr => sr.Client)
                .Include(sr => sr.Car)
                .Include(sr => sr.LinkedRepair)
                .FirstOrDefaultAsync(sr => sr.Id == id);

            if (request == null)
                return NotFound();

            var result = new
            {
                request.Id,
                request.ServiceType,
                request.Description,
                request.Comment,
                request.Status,
                request.InternalNotes,
                request.CreatedOn,
                Client = new
                {
                    request.Client.Name,
                    request.Client.Email,
                    request.Client.Phone
                },
                Car = new
                {
                    request.Car.Brand,
                    request.Car.Model,
                    request.Car.Year,
                    request.Car.RegistrationNumber,
                    request.Car.VIN
                },
                LinkedRepair = request.LinkedRepair != null ? new
                {
                    request.LinkedRepair.Id,
                    request.LinkedRepair.Status,
                    request.LinkedRepair.Price
                } : null
            };

            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id, 
            [FromBody] StatusUpdateModel model,
            [FromServices] EmailService emailService)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (request == null)
                return NotFound();

            // Запазваме стария статус за email нотификацията
            var oldStatus = request.Status;
            
            // Актуализираме статуса
            request.Status = model.Status;
            request.InternalNotes = model.InternalNotes;
            
            await _context.SaveChangesAsync();

            // Изпращаме email ако клиентът има имейл и статусът се променя
            if (!string.IsNullOrEmpty(request.Client.Email) && oldStatus != model.Status)
            {
                // Изпращаме email само за важни промени в статуса
                if (model.Status == "InProgress" || model.Status == "Completed" || model.Status == "Cancelled")
                {
                    await emailService.SendStatusUpdateEmailAsync(
                        request.Client.Email,
                        request.Client.Name,
                        request.Id,
                        request.ServiceType,
                        oldStatus,
                        model.Status,
                        model.InternalNotes
                    );
                }
            }

            return Ok(new { message = "Статусът беше актуализиран успешно!" });
        }

        [HttpPut("{id}/comment")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CommentUpdateModel model)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            request.Comment = model.Comment;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Коментарът беше актуализиран успешно!" });
        }

        [HttpPut("{id}/internalnotes")]
        public async Task<IActionResult> UpdateInternalNotes(Guid id, [FromBody] InternalNotesUpdateModel model)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            request.InternalNotes = model.InternalNotes;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Вътрешните бележки бяха актуализирани успешно!" });
        }

        [HttpPost("{id}/start-repair")]
        public async Task<IActionResult> StartRepair(
            Guid id, 
            [FromServices] RepairService repairService,
            [FromServices] EmailService emailService)
        {
            try
            {
                // Вземаме информацията за заявката преди да стартираме ремонта
                var serviceRequest = await _context.ServiceRequests
                    .Include(sr => sr.Client)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sr => sr.Id == id);
                
                if (serviceRequest == null)
                {
                    return NotFound(new { message = "Заявката не беше намерена" });
                }
                
                var repair = await repairService.CreateFromServiceRequestAsync(id);
                
                if (repair == null)
                {
                    return NotFound(new { message = "Заявката не беше намерена" });
                }

                // Изпращаме имейл известие към клиента че работата е започнала
                if (!string.IsNullOrEmpty(serviceRequest.Client?.Email))
                {
                    await emailService.SendStatusUpdateEmailAsync(
                        serviceRequest.Client.Email,
                        serviceRequest.Client.Name,
                        serviceRequest.Id,
                        serviceRequest.ServiceType,
                        "Pending",
                        "InProgress",
                        "Нашият екип започна работа по Вашата заявка."
                    );
                }

                return Ok(new 
                { 
                    message = "Услугата беше започната успешно!",
                    repairId = repair.Id.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartRepair: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Грешка при стартиране на услуга", error = ex.Message });
            }
        }
    }

    public class StatusUpdateModel
    {
        public string Status { get; set; } = string.Empty;
        public string? InternalNotes { get; set; }
    }

    public class CommentUpdateModel
    {
        public string Comment { get; set; } = string.Empty;
    }

    public class InternalNotesUpdateModel
    {
        public string? InternalNotes { get; set; }
    }
}
