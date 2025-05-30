using backendcafe.DTO;

namespace backendcafe.Services
{
    public interface ITableService
    {
        Task<List<TableReadDTO>> GetAllTablesAsync();
        Task<List<TableReadDTO>> GetTablesByBranchAsync(int branchId);
        Task<TableReadDTO> GetTableByIdAsync(int id);
        Task<TableReadDTO> CreateTableAsync(TableCreateDTO tableDto);
        Task<TableReadDTO> UpdateTableAsync(int id, TableUpdateDTO tableDto);
        Task<bool> DeleteTableAsync(TableDeleteDTO tableDto);
        Task<List<TableReadDTO>> GetAvailableTablesAsync(TableAvailabilityCheckDTO checkDto);
    }
}