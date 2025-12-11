using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project.Areas.Admin.Services
{
    public class EmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<bool> CreateAsync(Employee employee, string password)
        {
            try
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                employee.CreatedOn = DateTime.UtcNow;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Employee employee, string? newPassword = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(newPassword))
                {
                    employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }
                
                _context.Employees.Update(employee);
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
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null) return false;

                _context.Employees.Remove(employee);
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
