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
        private readonly IMidtransService _midtransService;

        public TransactionService(ApplicationDbContext context, IMidtransService midtransService)
        {
            _context = context;
            _midtransService = midtransService;
        }

        public async Task<TransactionResponseDTO> CreateTransactionWithCartItemsAsync(TransactionCreateDTO transactionDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transactionCode = GenerateTransactionCode();
                var newTransaction = new Transaction
                {
                    Name = transactionDto.Name,
                    TransactionCode = transactionCode,
                    Total = 0, 
                    BranchId = transactionDto.BranchId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = transactionDto.CreatedBy
                };

                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                int totalAmount = 0;
                foreach (var cartItem in transactionDto.CartItems)
                {
                    var product = await _context.Products
                        .Where(p => p.Id == cartItem.ProductId && p.BranchId == transactionDto.BranchId)
                        .FirstOrDefaultAsync();

                    if (product == null)
                        throw new Exception($"Product with ID {cartItem.ProductId} not found in branch {transactionDto.BranchId}");

                    if (!product.IsActive)
                        throw new Exception($"Product {product.Name} is not active");

                    if (product.Stock < cartItem.Quantity)
                        throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {cartItem.Quantity}");

                    var newCartItem = new Cart
                    {
                        BranchId = transactionDto.BranchId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        TransactionId = newTransaction.Id,
                    };

                    _context.Carts.Add(newCartItem);

                    product.Stock -= cartItem.Quantity;

                    totalAmount += product.Price * cartItem.Quantity;
                }

                newTransaction.Total = totalAmount;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

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

        public async Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentRequestDTO paymentRequest)
        {
            try
            {
                var transaction = await _context.Transactions
                    .Include(t => t.Carts)
                    .ThenInclude(c => c.Product)
                    .FirstOrDefaultAsync(t => t.Id == paymentRequest.TransactionId);

                if (transaction == null)
                    throw new Exception("Transaction not found");

                if (transaction.Status != "Pending")
                    throw new Exception("Transaction is not in pending status");

                // Prepare item details for Midtrans
                var itemDetails = new List<MidtransItemDetailsDTO>();
                foreach (var cartItem in transaction.Carts)
                {
                    itemDetails.Add(new MidtransItemDetailsDTO
                    {
                        Id = cartItem.ProductId.ToString(),
                        Name = cartItem.Product.Name,
                        Price = cartItem.Product.Price,
                        Quantity = cartItem.Quantity
                    });
                }

                // Create Midtrans transaction request
                var midtransRequest = new MidtransCreateTransactionDTO
                {
                    TransactionDetails = new MidtransTransactionDetailsDTO
                    {
                        OrderId = transaction.TransactionCode,
                        GrossAmount = transaction.Total
                    },
                    CustomerDetails = new MidtransCustomerDetailsDTO
                    {
                        FirstName = paymentRequest.CustomerName,
                        Email = paymentRequest.CustomerEmail,
                        Phone = paymentRequest.CustomerPhone
                    },
                    ItemDetails = itemDetails,
                    EnabledPayments = paymentRequest.EnabledPayments?.Count > 0 
                        ? paymentRequest.EnabledPayments 
                        : new List<string> { "credit_card", "bni_va", "bca_va", "bri_va", "echannel", "permata_va", "other_va", "gopay", "shopeepay" },
                    Callbacks = new MidtransCallbacksDTO
                    {
                        Finish = paymentRequest.FinishUrl,
                        Error = paymentRequest.ErrorUrl,
                        Pending = paymentRequest.PendingUrl
                    }
                };

                // Get Snap token from Midtrans
                var snapResponse = await _midtransService.CreateSnapTokenAsync(midtransRequest);

                // Update transaction status to "Waiting Payment"
                transaction.Status = "Waiting Payment";
                await _context.SaveChangesAsync();

                return new PaymentResponseDTO
                {
                    SnapToken = snapResponse.Token,
                    RedirectUrl = snapResponse.RedirectUrl,
                    OrderId = transaction.TransactionCode,
                    TransactionId = transaction.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create payment: {ex.Message}");
            }
        }

        public async Task<TransactionResponseDTO> HandlePaymentNotificationAsync(MidtransNotificationDTO notification)
        {
            try
            {
                // Verify notification signature
                var isValid = await _midtransService.VerifyNotificationAsync(notification);
                if (!isValid)
                    throw new Exception("Invalid notification signature");

                // Find transaction by order ID (transaction code)
                var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.TransactionCode == notification.OrderId);

                if (transaction == null)
                    throw new Exception("Transaction not found");

                // Update transaction status based on Midtrans status
                var newStatus = MapMidtransStatusToTransactionStatus(notification.TransactionStatus, notification.FraudStatus);
                
                if (transaction.Status != newStatus)
                {
                    transaction.Status = newStatus;
                    
                    // If payment is successful, you might want to do additional processing
                    if (newStatus == "Paid")
                    {
                        // Add any additional logic for successful payment
                        // e.g., send email confirmation, update inventory, etc.
                    }
                    else if (newStatus == "Failed" || newStatus == "Cancelled")
                    {
                        // Restore product stock if payment failed
                        await RestoreProductStockAsync(transaction.Id);
                    }

                    await _context.SaveChangesAsync();
                }

                return await GetTransactionByIdAsync(transaction.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to handle payment notification: {ex.Message}");
            }
        }

        public async Task<TransactionResponseDTO> GetTransactionByOrderIdAsync(string orderId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.TransactionCode == orderId)
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

            transaction.CartItems = await GetCartItemsByTransactionId(transaction.Id);

            return transaction;
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

        private async Task RestoreProductStockAsync(int transactionId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.TransactionId == transactionId)
                .ToListAsync();

            foreach (var cartItem in cartItems)
            {
                cartItem.Product.Stock += cartItem.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        private string MapMidtransStatusToTransactionStatus(string transactionStatus, string fraudStatus)
        {
            return transactionStatus?.ToLower() switch
            {
                "capture" => fraudStatus == "challenge" ? "Challenge" : "Paid",
                "settlement" => "Paid",
                "pending" => "Waiting Payment",
                "deny" => "Failed",
                "cancel" => "Cancelled",
                "expire" => "Expired",
                "failure" => "Failed",
                _ => "Unknown"
            };
        }

        private string GenerateTransactionCode()
        {
            return $"TRX-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}