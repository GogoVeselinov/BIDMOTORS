using Project.Models.Entities;
using Project.Models.ViewModels.Account;

namespace Project.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Client? Client)> RegisterClientAsync(RegisterViewModel model);
        Task<(bool Success, string Message, Client? Client)> LoginClientAsync(LoginViewModel model);
        Task<(bool Success, string Message, Employee? Employee)> LoginEmployeeAsync(LoginViewModel model);
        Task<Client?> GetClientByEmailAsync(string email);
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
