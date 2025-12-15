using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;
using Project.Models.ViewModels.Services;

namespace Project.Areas.Admin.Services
{
    public class ServiceTypeService
    {
        private readonly ApplicationDbContext _context;

        public ServiceTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Entity methods (за backward compatibility)
        public async Task<List<ServiceType>> GetAllAsync()
        {
            return await _context.ServiceTypes
                .OrderBy(st => st.Name)
                .ToListAsync();
        }

        public async Task<ServiceType?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceTypes.FindAsync(id);
        }

        // ViewModel methods (нови)
        public async Task<List<ServiceTypeListItemViewModel>> GetAllViewModelsAsync()
        {
            return await _context.ServiceTypes
                .OrderBy(st => st.Name)
                .Select(st => new ServiceTypeListItemViewModel
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description,
                    IsActive = true, // TODO: Add IsActive to entity
                    CreatedOn = st.CreatedOn
                })
                .ToListAsync();
        }

        public async Task<ServiceTypeDetailsViewModel?> GetByIdViewModelAsync(Guid id)
        {
            var entity = await _context.ServiceTypes
                .Include(st => st.Tasks)
                .Include(st => st.Parts)
                .FirstOrDefaultAsync(st => st.Id == id);
            
            if (entity == null) return null;

            return new ServiceTypeDetailsViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = true, // TODO: Add IsActive to entity
                CreatedOn = entity.CreatedOn,
                Tasks = entity.Tasks.OrderBy(t => t.OrderIndex).Select(t => new ServiceTypeTaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Notes = t.Notes,
                    OrderIndex = t.OrderIndex,
                    IsCompleted = t.IsCompleted
                }).ToList(),
                Parts = entity.Parts.Select(p => new ServiceTypePartViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Url = p.Url,
                    Supplier = p.Supplier,
                    Notes = p.Notes
                }).ToList()
            };
        }

        public async Task<bool> CreateFromViewModelAsync(CreateServiceTypeViewModel model)
        {
            try
            {
                var entity = new ServiceType
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    CreatedOn = DateTime.UtcNow
                };

                _context.ServiceTypes.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateFromViewModelAsync(UpdateServiceTypeViewModel model)
        {
            try
            {
                var entity = await _context.ServiceTypes.FindAsync(model.Id);
                if (entity == null) return false;

                entity.Name = model.Name;
                entity.Description = model.Description;

                _context.ServiceTypes.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateAsync(ServiceType serviceType)
        {
            try
            {
                serviceType.CreatedOn = DateTime.UtcNow;
                _context.ServiceTypes.Add(serviceType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(ServiceType serviceType)
        {
            try
            {
                _context.ServiceTypes.Update(serviceType);
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
                var serviceType = await _context.ServiceTypes.FindAsync(id);
                if (serviceType == null) return false;

                _context.ServiceTypes.Remove(serviceType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ServiceTypeTask methods
        public async Task<bool> AddTaskAsync(Guid serviceTypeId, string title, string? notes = null)
        {
            try
            {
                var maxOrder = await _context.ServiceTypeTasks
                    .Where(t => t.ServiceTypeId == serviceTypeId)
                    .MaxAsync(t => (int?)t.OrderIndex) ?? -1;

                var task = new ServiceTypeTask
                {
                    ServiceTypeId = serviceTypeId,
                    Title = title,
                    Notes = notes,
                    OrderIndex = maxOrder + 1,
                    CreatedOn = DateTime.UtcNow
                };

                _context.ServiceTypeTasks.Add(task);
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
                var task = await _context.ServiceTypeTasks.FindAsync(taskId);
                if (task == null) return false;

                _context.ServiceTypeTasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskCompletedAsync(Guid taskId, bool isCompleted)
        {
            try
            {
                var task = await _context.ServiceTypeTasks.FindAsync(taskId);
                if (task == null) return false;

                task.IsCompleted = isCompleted;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ServiceTypePart methods
        public async Task<bool> AddPartAsync(Guid serviceTypeId, string title, string url, string? supplier = null, string? notes = null)
        {
            try
            {
                var part = new ServiceTypePart
                {
                    ServiceTypeId = serviceTypeId,
                    Title = title,
                    Url = url,
                    Supplier = supplier,
                    Notes = notes,
                    CreatedOn = DateTime.UtcNow
                };

                _context.ServiceTypeParts.Add(part);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePartAsync(Guid partId)
        {
            try
            {
                var part = await _context.ServiceTypeParts.FindAsync(partId);
                if (part == null) return false;

                _context.ServiceTypeParts.Remove(part);
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
