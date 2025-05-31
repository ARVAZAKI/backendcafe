using System;
using backendcafe.Models;
using backendcafe.Utils;

namespace backendcafe.Data.Seeders
{
    public static class TableSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Tables.Any())
            {
                var tables = new List<Table>();

                // Tables untuk Jakarta Branch (BranchId = 1)
                // Table 2 orang
                for (int i = 1; i <= 8; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"J{i:D2}",
                        Capacity = 2,
                        IsAvailable = true,
                        BranchId = 1,
                        Description = "Meja untuk 2 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Table 4 orang
                for (int i = 9; i <= 16; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"J{i:D2}",
                        Capacity = 4,
                        IsAvailable = true,
                        BranchId = 1,
                        Description = "Meja untuk 4 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Table 6 orang
                for (int i = 17; i <= 20; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"J{i:D2}",
                        Capacity = 6,
                        IsAvailable = true,
                        BranchId = 1,
                        Description = "Meja untuk 6 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Tables untuk Bandung Branch (BranchId = 2)
                // Table 2 orang
                for (int i = 1; i <= 6; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"B{i:D2}",
                        Capacity = 2,
                        IsAvailable = true,
                        BranchId = 2,
                        Description = "Meja untuk 2 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Table 4 orang
                for (int i = 7; i <= 12; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"B{i:D2}",
                        Capacity = 4,
                        IsAvailable = true,
                        BranchId = 2,
                        Description = "Meja untuk 4 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Table 6 orang
                for (int i = 13; i <= 15; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"B{i:D2}",
                        Capacity = 6,
                        IsAvailable = true,
                        BranchId = 2,
                        Description = "Meja untuk 6 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Table 8 orang (VIP)
                for (int i = 16; i <= 18; i++)
                {
                    tables.Add(new Table
                    {
                        TableNumber = $"B{i:D2}",
                        Capacity = 8,
                        IsAvailable = true,
                        BranchId = 2,
                        Description = "Meja VIP untuk 8 orang",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Tables.AddRange(tables);
                await context.SaveChangesAsync();
            }
        }
    }
}