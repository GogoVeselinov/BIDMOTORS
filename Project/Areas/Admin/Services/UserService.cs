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

        public async Task<(bool success, string message)> ConvertClientToEmployeeAsync(Guid clientId, string role)
        {
            try
            {
                var client = await _context.Clients
                    .Include(c => c.Notifications)
                    .FirstOrDefaultAsync(c => c.Id == clientId);

                if (client == null)
                {
                    return (false, "Клиентът не е намерен");
                }

                // Проверка дали вече съществува служител със същия имейл
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == client.Email);

                if (existingEmployee != null)
                {
                    return (false, "Вече съществува служител с този имейл");
                }

                // Създаване на нов служител
                var employee = new Employee
                {
                    Id = Guid.NewGuid(),
                    Name = client.Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    PasswordHash = client.PasswordHash ?? string.Empty,
                    Role = role,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Employees.Add(employee);

                // Преместване на нотификациите
                foreach (var notification in client.Notifications)
                {
                    notification.ClientId = null;
                    notification.EmployeeId = employee.Id;
                }

                // Изтриване на клиента
                _context.Clients.Remove(client);

                await _context.SaveChangesAsync();

                return (true, "Акаунтът е преместен успешно в таблицата на служителите");
            }
            catch (Exception ex)
            {
                return (false, $"Грешка при преместване на акаунта: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ConvertEmployeeToClientAsync(Guid employeeId)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Notifications)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (employee == null)
                {
                    return (false, "Служителят не е намерен");
                }

                // Проверка дали вече съществува клиент със същия имейл
                var existingClient = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == employee.Email);

                if (existingClient != null)
                {
                    return (false, "Вече съществува клиент с този имейл");
                }

                // Създаване на нов клиент
                var client = new Client
                {
                    Id = Guid.NewGuid(),
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    PasswordHash = employee.PasswordHash,
                    Role = "User",
                    IsGuest = false,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Clients.Add(client);

                // Преместване на нотификациите
                foreach (var notification in employee.Notifications)
                {
                    notification.EmployeeId = null;
                    notification.ClientId = client.Id;
                }

                // Изтриване на служителя
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();

                return (true, "Акаунтът е преместен успешно в таблицата на клиентите");
            }
            catch (Exception ex)
            {
                return (false, $"Грешка при преместване на акаунта: {ex.Message}");
            }
        }
    }
}
