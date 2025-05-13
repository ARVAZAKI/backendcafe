using backendcafe.Models;

namespace backendcafe.DTO
{
    public class TransactionCreateDTO
    {
        public string Name { get; set; }
        public int BranchId { get; set; }
        public string CreatedBy { get; set; } = "customer";
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }

    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class TransactionResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TransactionCode { get; set; }
        public int Total { get; set; }
        public string Status { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public List<CartResponseDTO> CartItems { get; set; } = new List<CartResponseDTO>();
    }

    public class CartResponseDTO
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int Subtotal { get; set; }
    }

    public class TransactionStatusUpdateDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}