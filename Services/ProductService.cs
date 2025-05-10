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
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductResponseDTO>> GetProductsByBranchAndCategoryAsync(int branchId, int? categoryId)
        {
            var query = _context.Products
                .Where(p => p.BranchId == branchId && p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return await query
                .Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock,
                    Price = p.Price,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    BranchId = p.BranchId
                })
                .ToListAsync();
        }

        public async Task<ProductResponseDTO> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stock = p.Stock,
                    Price = p.Price,
                    Description = p.Description,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    BranchId = p.BranchId
                })
                .FirstOrDefaultAsync();

            if (product == null)
                throw new Exception("Product not found");

            return product;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(ProductCreateDTO productDto)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == productDto.BranchId);
            if (!branchExists)
                throw new ArgumentException("Branch not found");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException("Category not found");

            var product = new Product
            {
                Name = productDto.Name,
                Stock = productDto.Stock,
                Price = productDto.Price,
                Description = productDto.Description,
                IsActive = productDto.IsActive,
                CategoryId = productDto.CategoryId,
                BranchId = productDto.BranchId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Stock = product.Stock,
                Price = product.Price,
                Description = product.Description,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                BranchId = product.BranchId
            };
        }

        public async Task<ProductResponseDTO> UpdateProductAsync(int id, ProductUpdateDTO productDto)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == productDto.BranchId);
            if (!branchExists)
                throw new ArgumentException("Branch not found");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException("Category not found");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            product.Name = productDto.Name;
            product.Stock = productDto.Stock;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.IsActive = productDto.IsActive;
            product.CategoryId = productDto.CategoryId;
            product.BranchId = productDto.BranchId;

            await _context.SaveChangesAsync();

            return new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Stock = product.Stock,
                Price = product.Price,
                Description = product.Description,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                BranchId = product.BranchId
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}