using System;

using backendcafe.Models;
using backendcafe.Utils;

namespace backendcafe.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "superadmin",
                        Email = "superadmin@example.com",
                        PasswordHash = HashUtility.ComputeHash("admin123"),
                        Role = Role.SuperAdmin
                    },
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = HashUtility.ComputeHash("admin123"),
                        Role = Role.Admin
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}