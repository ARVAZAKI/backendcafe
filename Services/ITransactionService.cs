   using backendcafe.DTO;     
   namespace backendcafe.Services{
    public interface ITransactionService
    {
        Task<TransactionResponseDTO> CreateTransactionWithCartItemsAsync(TransactionCreateDTO transactionDto);
        Task<List<TransactionResponseDTO>> GetAllTransactionsAsync();
        Task<TransactionResponseDTO> GetTransactionByIdAsync(int id);
        Task<TransactionResponseDTO> UpdateTransactionStatusAsync(int id, string status);
         Task<PaymentResponseDTO> CreatePaymentAsync(CreatePaymentRequestDTO paymentRequest);
        Task<TransactionResponseDTO> HandlePaymentNotificationAsync(MidtransNotificationDTO notification);
        Task<TransactionResponseDTO> GetTransactionByOrderIdAsync(string orderId);
    }
    }

   