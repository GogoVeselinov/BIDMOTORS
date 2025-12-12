using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;
using Project.Models.Enum;

namespace Project.Areas.Admin.Services
{
    public class ServiceService
    {
        private readonly ApplicationDbContext _context = null!;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // === SERVICE CRUD ===

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceType)
                .Include(s => s.ServiceRequest)
                    .ThenInclude(sr => sr.Client)
                .Include(s => s.ServiceRequest)
                    .ThenInclude(sr => sr.Car)
                .Include(s => s.AssignedEmployee)
                .Include(s => s.Tasks)
                .Include(s => s.PartLinks)
                .OrderByDescending(s => s.CreatedOn)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid id)
        {
            return await _context.Services
                .Include(s => s.ServiceType)
                .Include(s => s.ServiceRequest)
                    .ThenInclude(sr => sr.Client)
                .Include(s => s.ServiceRequest)
                    .ThenInclude(sr => sr.Car)
                .Include(s => s.AssignedEmployee)
                .Include(s => s.Tasks)
                    .ThenInclude(t => t.CompletedByEmployee)
                .Include(s => s.PartLinks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Service>> GetByServiceRequestIdAsync(Guid serviceRequestId)
        {
            return await _context.Services
                .Include(s => s.ServiceType)
                .Include(s => s.AssignedEmployee)
                .Include(s => s.Tasks)
                .Include(s => s.PartLinks)
                .Where(s => s.ServiceRequestId == serviceRequestId)
                .OrderBy(s => s.CreatedOn)
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(Service service)
        {
            try
            {
                service.CreatedOn = DateTime.UtcNow;
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Service service)
        {
            try
            {
                _context.Services.Update(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(Guid id, ServiceStatus status)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null) return false;

                service.Status = status;

                if (status == ServiceStatus.InProgress && !service.StartedOn.HasValue)
                {
                    service.StartedOn = DateTime.UtcNow;
                }
                else if (status == ServiceStatus.Completed && !service.CompletedOn.HasValue)
                {
                    service.CompletedOn = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AssignEmployeeAsync(Guid serviceId, Guid? employeeId)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null) return false;

                service.AssignedEmployeeId = employeeId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var service = await _context.Services
                    .Include(s => s.Tasks)
                    .Include(s => s.PartLinks)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (service == null) return false;

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // === SERVICE TASK CRUD ===

        public async Task<ServiceTask?> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.ServiceTasks
                .Include(t => t.CompletedByEmployee)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<bool> CreateTaskAsync(ServiceTask task)
        {
            try
            {
                task.CreatedOn = DateTime.UtcNow;
                _context.ServiceTasks.Add(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskAsync(ServiceTask task)
        {
            try
            {
                _context.ServiceTasks.Update(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleTaskCompletionAsync(Guid taskId, Guid? employeeId)
        {
            try
            {
                var task = await _context.ServiceTasks.FindAsync(taskId);
                if (task == null) return false;

                task.IsCompleted = !task.IsCompleted;
                task.CompletedOn = task.IsCompleted ? DateTime.UtcNow : null;
                task.CompletedByEmployeeId = task.IsCompleted ? employeeId : null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            try
            {
                var task = await _context.ServiceTasks.FindAsync(taskId);
                if (task == null) return false;

                _context.ServiceTasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // === SERVICE PART LINK CRUD ===

        public async Task<ServicePartLink?> GetPartLinkByIdAsync(Guid partLinkId)
        {
            return await _context.ServicePartLinks.FindAsync(partLinkId);
        }

        public async Task<bool> CreatePartLinkAsync(ServicePartLink partLink)
        {
            try
            {
                partLink.CreatedOn = DateTime.UtcNow;
                _context.ServicePartLinks.Add(partLink);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePartLinkAsync(ServicePartLink partLink)
        {
            try
            {
                partLink.CreatedOn = DateTime.UtcNow;
                _context.ServicePartLinks.Update(partLink);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePartLinkAsync(Guid partLinkId)
        {
            try
            {
                var partLink = await _context.ServicePartLinks.FindAsync(partLinkId);
                if (partLink == null) return false;

                _context.ServicePartLinks.Remove(partLink);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
