using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backendcafe.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryReadDTO>> GetCategoriesByBranchAsync(int branchId)
        {
            return await _context.Categories
                .Where(c => c.BranchId == branchId)
                .Select(c => new CategoryReadDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    BranchId = c.BranchId
                })
                .ToListAsync();
        }

        public async Task<CategoryReadDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryReadDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    BranchId = c.BranchId
                })
                .FirstOrDefaultAsync();

            if (category == null)
                throw new Exception("Category not found");

            return category;
        }

        public async Task<CategoryReadDTO> CreateCategoryAsync(CategoryCreateDTO categoryDto)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == categoryDto.BranchId);
            if (!branchExists)
                throw new ArgumentException("Branch not found");

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                BranchId = categoryDto.BranchId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryReadDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                BranchId = category.BranchId
            };
        }

        public async Task<CategoryReadDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDto)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == categoryDto.BranchId);
            if (!branchExists)
                throw new ArgumentException("Branch not found");

            var category = await _context.Categories.FindAsync(categoryDto.Id);
            if (category == null)
                throw new Exception("Category not found");

            category.CategoryName = categoryDto.CategoryName;
            category.BranchId = categoryDto.BranchId;

            await _context.SaveChangesAsync();

            return new CategoryReadDTO
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                BranchId = category.BranchId
            };
        }

        public async Task<bool> DeleteCategoryAsync(CategoryDeleteDTO categoryDto)
        {
            var category = await _context.Categories.FindAsync(categoryDto.Id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}