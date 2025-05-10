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
                        LogoUrl = "https://example.com/logos/jakarta.png",
                        BannerUrl = "https://example.com/banners/jakarta.png"
                    },
                    new Branch
                    {
                        BranchName = "Bandung Branch",
                        Address = "456 Dago St",
                        LogoUrl = "https://example.com/logos/bandung.png",
                        BannerUrl = null
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}