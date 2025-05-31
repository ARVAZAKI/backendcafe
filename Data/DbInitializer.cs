using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backendcafe.Data.Seeders;

namespace backendcafe.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            try
            {
                // Retry logic untuk memastikan database siap
                int retries = 5;
                int delayMs = 5000; // 5 detik
                bool canConnect = false;

                for (int i = 0; i < retries; i++)
                {
                    Console.WriteLine($"Mencoba koneksi ke database (percobaan {i + 1}/{retries})...");
                    canConnect = await context.Database.CanConnectAsync();
                    if (canConnect)
                        break;
                    
                    Console.WriteLine("Database belum siap, menunggu...");
                    await Task.Delay(delayMs);
                }

                if (!canConnect)
                {
                    Console.WriteLine("Database tidak dapat diakses, mencoba membuat...");
                    await context.Database.EnsureCreatedAsync();
                }
                else
                {
                    // Database ada, cek migrasi tertunda
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        Console.WriteLine($"Menerapkan {pendingMigrations.Count()} migrasi tertunda...");
                        await context.Database.MigrateAsync();
                    }
                    else
                    {
                        Console.WriteLine("Database sudah up-to-date.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inisialisasi database: {ex.Message}");
                if (ex.Message.Contains("already exists"))
                {
                    Console.WriteLine("Database/tabel sudah ada, lanjutkan ke seeding...");
                }
                else
                {
                    throw;
                }
            }

        
            await BranchSeeder.SeedAsync(context);
            await UserSeeder.SeedAsync(context);
            await CategorySeeder.SeedAsync(context);
            await ProductSeeder.SeedAsync(context);
            await TableSeeder.SeedAsync(context);
        }
    }
}