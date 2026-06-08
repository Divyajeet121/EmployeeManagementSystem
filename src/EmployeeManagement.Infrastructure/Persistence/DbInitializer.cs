using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var admin = new User
            {
                Username = "admin",
                PasswordHash = passwordHasher.Hash("Admin@123"),
                Role = UserRole.Admin,
                IsActive = true
            };

            var standardUser = new User
            {
                Username = "john.user",
                PasswordHash = passwordHasher.Hash("User@123"),
                Role = UserRole.User,
                IsActive = true
            };

            context.Users.AddRange(admin, standardUser);
            await context.SaveChangesAsync();
        }
    }
}
