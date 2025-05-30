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
        private readonly IS3Service _s3Service; // Tambahkan field yang hilang

        public ProductService(ApplicationDbContext context, IS3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service; // Perbaiki assignment
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
                    BranchId = p.BranchId,
                    ImageUrl = p.ImageUrl
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
                    BranchId = p.BranchId,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (product == null)
                throw new Exception("Product not found");

            return product;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(ProductCreateDTO productDto)
        {
            
            // Validasi foreign key existence
            if (!await _context.Branches.AnyAsync(b => b.Id == productDto.BranchId))
                throw new ArgumentException("Branch not found");

            if (!await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId))
                throw new ArgumentException("Category not found");

            // Handle image upload (opsional)
            string? imageUrl = null;
            if (productDto.ImageFile != null)
            {
                var fileName = $"products/{Guid.NewGuid()}_{Path.GetFileName(productDto.ImageFile.FileName)}";
                imageUrl = await _s3Service.UploadFileAsync(productDto.ImageFile, fileName);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Stock = productDto.Stock,
                Price = productDto.Price,
                Description = productDto.Description ?? string.Empty,
                IsActive = productDto.IsActive,
                CategoryId = productDto.CategoryId,
                BranchId = productDto.BranchId,
                ImageUrl = imageUrl
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
                BranchId = product.BranchId,
                ImageUrl = product.ImageUrl
            };
        }


        public async Task<ProductResponseDTO> UpdateProductAsync(int id, ProductUpdateDTO productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new ArgumentException("Product not found");

            // Validasi foreign key existence
            if (!await _context.Branches.AnyAsync(b => b.Id == productDto.BranchId))
                throw new ArgumentException("Branch not found");

            if (!await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId))
                throw new ArgumentException("Category not found");

            // Handle image update
            if (productDto.ImageFile != null)
            {
                // Hapus gambar lama dari S3 jika ada
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _s3Service.DeleteFileAsync(product.ImageUrl);
                }

                var fileName = $"products/{Guid.NewGuid()}_{Path.GetFileName(productDto.ImageFile.FileName)}";
                product.ImageUrl = await _s3Service.UploadFileAsync(productDto.ImageFile, fileName);
            }

            // Update properti produk
            product.Name = productDto.Name;
            product.Stock = productDto.Stock;
            product.Price = productDto.Price;
            product.Description = productDto.Description ?? string.Empty;
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
                BranchId = product.BranchId,
                ImageUrl = product.ImageUrl
            };
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            // Delete image from S3 if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _s3Service.DeleteFileAsync(product.ImageUrl);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}