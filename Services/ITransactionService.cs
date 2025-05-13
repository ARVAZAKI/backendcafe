   using backendcafe.DTO;     
   namespace backendcafe.Services{
public interface ITransactionService
    {
        Task<TransactionResponseDTO> CreateTransactionWithCartItemsAsync(TransactionCreateDTO transactionDto);
        Task<List<TransactionResponseDTO>> GetAllTransactionsAsync();
        Task<TransactionResponseDTO> GetTransactionByIdAsync(int id);
        Task<TransactionResponseDTO> UpdateTransactionStatusAsync(int id, string status);
    }

   }
   