using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/servicerequests")]
    [ApiController]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.Client)
                .Include(sr => sr.Car)
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
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] StatusUpdateModel model)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            request.Status = model.Status;
            request.InternalNotes = model.InternalNotes;
            
            await _context.SaveChangesAsync();

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
}
