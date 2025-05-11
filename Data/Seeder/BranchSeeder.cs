using System;

using backendcafe.Models;
using backendcafe.Utils;

namespace backendcafe.Data.Seeders
{
    public static class BranchSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch
                    {
                        BranchName = "Jakarta Branch",
                        Address = "123 Sudirman St",
                    },
                    new Branch
                    {
                        BranchName = "Bandung Branch",
                        Address = "456 Dago St",
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}