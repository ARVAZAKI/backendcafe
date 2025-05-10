using backendcafe.Data.Seeders;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

        
            await BranchSeeder.SeedAsync(context);
            await UserSeeder.SeedAsync(context);
            await CategorySeeder.SeedAsync(context);
            await ProductSeeder.SeedAsync(context);
        }
    }
}