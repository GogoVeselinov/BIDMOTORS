using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Services
{
    public class ServiceTypeService
    {
        private readonly ApplicationDbContext _context;

        public ServiceTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
