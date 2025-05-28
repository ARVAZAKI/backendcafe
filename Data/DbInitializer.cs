using backendcafe.Data.Seeders;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            try
            {
                // Check if database exists and can be connected to
                var canConnect = await context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    Console.WriteLine("Database not accessible, creating...");
                    await context.Database.EnsureCreatedAsync();
                }
                else
                {
                    // Database exists, check for pending migrations
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                        await context.Database.MigrateAsync();
                    }
                    else
                    {
                        Console.WriteLine("Database is up to date.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                
                // Try to handle common scenarios
                if (ex.Message.Contains("already exists"))
                {
                    Console.WriteLine("Database/tables already exist, continuing with seeding...");
                }
                else
                {
                    throw; // Re-throw if it's not a "already exists" error
                }
            }

            // Run seeders
            try
            {
                Console.WriteLine("Starting data seeding...");
                await BranchSeeder.SeedAsync(context);
                await UserSeeder.SeedAsync(context);
                await CategorySeeder.SeedAsync(context);
                await ProductSeeder.SeedAsync(context);
                Console.WriteLine("Data seeding completed successfully.");
            }
            catch (Exception seedEx)
            {
                Console.WriteLine($"Seeding error: {seedEx.Message}");
                throw; // Re-throw seeding errors
            }
        }
    }
}