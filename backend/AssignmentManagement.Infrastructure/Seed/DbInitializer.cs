using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using AssignmentManagement.Infrastructure.Persistence;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new User
            {
                FullName = "System Admin",
                Email = "admin@school.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = Role.Admin,
                IsActive = true
            },

            new User
            {
                FullName = "John Teacher",
                Email = "teacher@school.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                Role = Role.Teacher,
                IsActive = true
            },

            new User
            {
                FullName = "Alex Student",
                Email = "student@school.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                Role = Role.Student,
                IsActive = true
            }
        };

        await context.Users.AddRangeAsync(users);

        await context.SaveChangesAsync();
    }
}