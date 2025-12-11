using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models.Entities;

namespace Project;

public static class SeedAdmin
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Проверяваме дали вече има админ
        var existingAdmin = context.Employees.FirstOrDefault(e => e.Email == "admin@bidmotors.com");
        if (existingAdmin != null)
        {
            Console.WriteLine("Admin account already exists.");
            return;
        }

        // Създаваме admin акаунт
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var adminPassword = "Admin123!"; // Променете след първото влизане!
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

        var admin = new Employee
        {
            Id = adminId,
            Name = "Administrator",
            Email = "admin@bidmotors.com",
            Phone = "0888888888",
            PasswordHash = passwordHash,
            Role = "Admin",
            CreatedOn = new DateTime(2025, 12, 11, 0, 0, 0, DateTimeKind.Utc)
        };

        context.Employees.Add(admin);
        context.SaveChanges();

        Console.WriteLine("============================================");
        Console.WriteLine("Admin account created successfully!");
        Console.WriteLine("Email: admin@bidmotors.com");
        Console.WriteLine("Password: Admin123!");
        Console.WriteLine("Please change the password after first login.");
        Console.WriteLine("============================================");
    }
}
