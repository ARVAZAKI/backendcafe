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
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionResponseDTO> CreateTransactionWithCartItemsAsync(TransactionCreateDTO transactionDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create transaction first with initial data
                var transactionCode = GenerateTransactionCode();
                var newTransaction = new Transaction
                {
                    Name = transactionDto.Name,
                    TransactionCode = transactionCode,
                    Total = 0, // Will be updated after cart items are processed
                    BranchId = transactionDto.BranchId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    CreatedBy = transactionDto.CreatedBy
                };

                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                // 2. Process cart items
                int totalAmount = 0;
                foreach (var cartItem in transactionDto.CartItems)
                {
                    // Get product to validate and calculate price
                    var product = await _context.Products
                        .Where(p => p.Id == cartItem.ProductId && p.BranchId == transactionDto.BranchId)
                        .FirstOrDefaultAsync();

                    if (product == null)
                        throw new Exception($"Product with ID {cartItem.ProductId} not found in branch {transactionDto.BranchId}");

                    if (!product.IsActive)
                        throw new Exception($"Product {product.Name} is not active");

                    if (product.Stock < cartItem.Quantity)
                        throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {cartItem.Quantity}");

                    // Create cart item
                    var newCartItem = new Cart
                    {
                        BranchId = transactionDto.BranchId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        TransactionId = newTransaction.Id,
                    };

                    _context.Carts.Add(newCartItem);

                    // Update product stock
                    product.Stock -= cartItem.Quantity;

                    // Calculate total amount
                    totalAmount += product.Price * cartItem.Quantity;
                }

                // 3. Update transaction with total amount
                newTransaction.Total = totalAmount;
                await _context.SaveChangesAsync();

                // 4. Commit transaction
                await transaction.CommitAsync();

                // 5. Return response
                return new TransactionResponseDTO
                {
                    Id = newTransaction.Id,
                    Name = newTransaction.Name,
                    TransactionCode = newTransaction.TransactionCode,
                    Total = newTransaction.Total,
                    Status = newTransaction.Status,
                    BranchId = newTransaction.BranchId,
                    CreatedAt = newTransaction.CreatedAt,
                    CreatedBy = newTransaction.CreatedBy,
                    CartItems = await GetCartItemsByTransactionId(newTransaction.Id)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to create transaction: {ex.Message}");
            }
        }

        public async Task<List<TransactionResponseDTO>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    TransactionCode = t.TransactionCode,
                    Total = t.Total,
                    Status = t.Status,
                    BranchId = t.BranchId,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                })
                .ToListAsync();

            foreach (var transaction in transactions)
            {
                transaction.CartItems = await GetCartItemsByTransactionId(transaction.Id);
            }

            return transactions;
        }

        public async Task<TransactionResponseDTO> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    TransactionCode = t.TransactionCode,
                    Total = t.Total,
                    Status = t.Status,
                    BranchId = t.BranchId,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                })
                .FirstOrDefaultAsync();

            if (transaction == null)
                throw new Exception("Transaction not found");

            transaction.CartItems = await GetCartItemsByTransactionId(id);

            return transaction;
        }

        public async Task<TransactionResponseDTO> UpdateTransactionStatusAsync(int id, string status)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                throw new Exception("Transaction not found");

            transaction.Status = status;
            await _context.SaveChangesAsync();

            return new TransactionResponseDTO
            {
                Id = transaction.Id,
                Name = transaction.Name,
                TransactionCode = transaction.TransactionCode,
                Total = transaction.Total,
                Status = transaction.Status,
                BranchId = transaction.BranchId,
                CreatedAt = transaction.CreatedAt,
                CreatedBy = transaction.CreatedBy,
                CartItems = await GetCartItemsByTransactionId(id)
            };
        }

        private async Task<List<CartResponseDTO>> GetCartItemsByTransactionId(int transactionId)
        {
            return await _context.Carts
                .Where(c => c.TransactionId == transactionId)
                .Select(c => new CartResponseDTO
                {
                    Id = c.Id,
                    BranchId = c.BranchId,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductPrice = c.Product.Price,
                    Quantity = c.Quantity,
                    Subtotal = c.Product.Price * c.Quantity
                })
                .ToListAsync();
        }

        private string GenerateTransactionCode()
        {
            // Generate a unique transaction code, you can customize this as needed
            return $"TRX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}