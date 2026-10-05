using FixMyCampus.Api.Enums;
using FixMyCampus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCampus.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    FullName = "System Admin",
                    Email = "admin@hackathon.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },

                new User
                {
                    FullName = "Student Reporter",
                    Email = "user@hackathon.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = UserRole.Reporter,
                    CreatedAt = DateTime.UtcNow
                },

                new User
                {
                    FullName = "Sara Reporter",
                    Email = "sara@hackathon.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = UserRole.Reporter,
                    CreatedAt = DateTime.UtcNow
                },

                new User
                {
                    FullName = "Abebe Technician",
                    Email = "abebe@hackathon.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"),
                    Role = UserRole.Technician,
                    CreatedAt = DateTime.UtcNow
                },

                new User
                {
                    FullName = "Marta Technician",
                    Email = "marta@hackathon.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"),
                    Role = UserRole.Technician,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        if (!await context.Buildings.AnyAsync())
        {
            var buildings = new List<Building>
            {
                new Building
                {
                    Name = "Engineering Building"
                },

                new Building
                {
                    Name = "Computer Science Building"
                },

                new Building
                {
                    Name = "Main Library"
                },

                new Building
                {
                    Name = "Student Center"
                }
            };

            await context.Buildings.AddRangeAsync(buildings);
            await context.SaveChangesAsync();
        }
    }
}