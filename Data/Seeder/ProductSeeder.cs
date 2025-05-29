using backendcafe.Models;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Data.Seeders
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var jakartaDrinks = await context.Categories.FirstAsync(c => c.CategoryName == "Drinks" && c.Branch.BranchName == "Jakarta Branch");
                var jakartaFood = await context.Categories.FirstAsync(c => c.CategoryName == "Food" && c.Branch.BranchName == "Jakarta Branch");
                var bandungDrinks = await context.Categories.FirstAsync(c => c.CategoryName == "Drinks" && c.Branch.BranchName == "Bandung Branch");
                var bandungFood = await context.Categories.FirstAsync(c => c.CategoryName == "Food" && c.Branch.BranchName == "Bandung Branch");

                context.Products.AddRange(
                    new Product
                    {
                        Name = "Coffee",
                        Stock = 100,
                        Price = 25000,
                        Description = "Espresso-based coffee",
                        ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=400",
                        IsActive = true,
                        CategoryId = jakartaDrinks.Id,
                        BranchId = jakartaDrinks.BranchId
                    },
                    new Product
                    {
                        Name = "Burger",
                        Stock = 50,
                        Price = 45000,
                        Description = "Beef burger with cheese",
                        ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400",
                        IsActive = true,
                        CategoryId = jakartaFood.Id,
                        BranchId = jakartaFood.BranchId
                    },
                    new Product
                    {
                        Name = "Tea",
                        Stock = 80,
                        Price = 20000,
                        Description = "Hot green tea",
                        ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400",
                        IsActive = true,
                        CategoryId = bandungDrinks.Id,
                        BranchId = bandungDrinks.BranchId
                    },
                    new Product
                    {
                        Name = "Pasta",
                        Stock = 30,
                        Price = 55000,
                        Description = "Spaghetti carbonara",
                        ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3dbc353d2e5?w=400",
                        IsActive = true,
                        CategoryId = bandungFood.Id,
                        BranchId = bandungFood.BranchId
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}