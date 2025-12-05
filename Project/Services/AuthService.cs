using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;
using Project.Models.ViewModels.Account;
using Project.Services.Interfaces;

namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Client? Client)> RegisterClientAsync(RegisterViewModel model)
        {
            // Проверка дали имейлът вече съществува
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == model.Email);

            if (existingClient != null)
            {
                return (false, "Потребител с този имейл вече съществува", null);
            }

            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == model.Email);

            if (existingEmployee != null)
            {
                return (false, "Този имейл е регистриран като служител", null);
            }

            // Създаване на нов клиент
            var client = new Client
            {
                Name = model.FullName,
                Email = model.Email,
                Phone = string.Empty, // Телефонът се добавя след регистрация
                PasswordHash = HashPassword(model.Password),
                CreatedOn = DateTime.UtcNow
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return (true, "Регистрацията е успешна", client);
        }

        public async Task<(bool Success, string Message, Client? Client)> LoginClientAsync(LoginViewModel model)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == model.Email);

            if (client == null)
            {
                return (false, "Невалиден имейл или парола", null);
            }

            if (string.IsNullOrEmpty(client.PasswordHash))
            {
                return (false, "Този акаунт няма зададена парола", null);
            }

            if (!VerifyPassword(model.Password, client.PasswordHash))
            {
                return (false, "Невалиден имейл или парола", null);
            }

            return (true, "Влизането е успешно", client);
        }

        public async Task<(bool Success, string Message, Employee? Employee)> LoginEmployeeAsync(LoginViewModel model)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == model.Email);

            if (employee == null)
            {
                return (false, "Невалиден имейл или парола", null);
            }

            if (!VerifyPassword(model.Password, employee.PasswordHash))
            {
                return (false, "Невалиден имейл или парола", null);
            }

            return (true, "Влизането е успешно", employee);
        }

        public async Task<Client?> GetClientByEmailAsync(string email)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
