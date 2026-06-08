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

            context.Employees.AddRange(
                new Employee
                {
                    EmployeeCode = "EMP-1001",
                    FirstName = "Aditi",
                    LastName = "Sharma",
                    Email = "aditi.sharma@example.com",
                    Department = "Engineering",
                    Designation = "Software Engineer",
                    JoiningDate = new DateTime(2022, 3, 14, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    CreatedBy = admin.UserId,
                    CreatedOn = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeCode = "EMP-1002",
                    FirstName = "Rohan",
                    LastName = "Mehta",
                    Email = "rohan.mehta@example.com",
                    Department = "Engineering",
                    Designation = "Senior Software Engineer",
                    JoiningDate = new DateTime(2021, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    CreatedBy = admin.UserId,
                    CreatedOn = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeCode = "EMP-1003",
                    FirstName = "Neha",
                    LastName = "Verma",
                    Email = "neha.verma@example.com",
                    Department = "Human Resources",
                    Designation = "HR Executive",
                    JoiningDate = new DateTime(2023, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    CreatedBy = admin.UserId,
                    CreatedOn = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeCode = "EMP-1004",
                    FirstName = "Karan",
                    LastName = "Gupta",
                    Email = "karan.gupta@example.com",
                    Department = "Finance",
                    Designation = "Accountant",
                    JoiningDate = new DateTime(2020, 11, 23, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = false,
                    CreatedBy = admin.UserId,
                    CreatedOn = DateTime.UtcNow
                },
                new Employee
                {
                    EmployeeCode = "EMP-1005",
                    FirstName = "Priya",
                    LastName = "Nair",
                    Email = "priya.nair@example.com",
                    Department = "Engineering",
                    Designation = "QA Engineer",
                    JoiningDate = new DateTime(2022, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    CreatedBy = admin.UserId,
                    CreatedOn = DateTime.UtcNow
                });

            await context.SaveChangesAsync();
        }
    }
}
