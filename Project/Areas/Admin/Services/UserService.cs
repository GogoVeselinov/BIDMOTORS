using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.Cars)
                .Include(c => c.ServiceRequests)
                .Include(c => c.Repairs)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _context.Clients
                .Include(c => c.Cars)
                .Include(c => c.ServiceRequests)
                .Include(c => c.Repairs)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> GetTotalCarsCountAsync(Guid clientId)
        {
            return await _context.Cars.CountAsync(c => c.ClientId == clientId);
        }

        public async Task<int> GetTotalServiceRequestsCountAsync(Guid clientId)
        {
            return await _context.ServiceRequests.CountAsync(sr => sr.ClientId == clientId);
        }

        public async Task<int> GetTotalRepairsCountAsync(Guid clientId)
        {
            return await _context.Repairs.CountAsync(r => r.ClientId == clientId);
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            try
            {
                _context.Clients.Update(client);
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
