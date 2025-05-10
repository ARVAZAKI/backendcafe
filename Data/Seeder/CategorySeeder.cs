using backendcafe.Models;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Data.Seeders
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var jakartaBranch = await context.Branches.FirstAsync(b => b.BranchName == "Jakarta Branch");
                var bandungBranch = await context.Branches.FirstAsync(b => b.BranchName == "Bandung Branch");

                context.Categories.AddRange(
                    new Category
                    {
                        CategoryName = "Drinks",
                        BranchId = jakartaBranch.Id
                    },
                    new Category
                    {
                        CategoryName = "Food",
                        BranchId = jakartaBranch.Id
                    },
                    new Category
                    {
                        CategoryName = "Drinks",
                        BranchId = bandungBranch.Id
                    },
                    new Category
                    {
                        CategoryName = "Food",
                        BranchId = bandungBranch.Id
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}