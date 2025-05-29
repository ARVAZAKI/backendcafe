using backendcafe.DTO;

namespace backendcafe.Services
{
    public interface ITableReservationService
    {
        Task<List<TableReservationReadDTO>> GetAllReservationsAsync();
        Task<List<TableReservationReadDTO>> GetReservationsByBranchAsync(int branchId);
        Task<List<TableReservationReadDTO>> GetReservationsByCustomerAsync(string customerName, string? customerPhone = null);
        Task<TableReservationReadDTO> GetReservationByIdAsync(int id);
        Task<TableReservationReadDTO> GetReservationByCodeAsync(string reservationCode);
        Task<TableReservationReadDTO> CreateReservationAsync(TableReservationCreateDTO reservationDto);
        Task<TableReservationReadDTO> UpdateReservationAsync(int id, TableReservationUpdateDTO reservationDto);
        Task<bool> DeleteReservationAsync(TableReservationDeleteDTO reservationDto);
        Task<List<TableReadDTO>> CheckTableAvailabilityAsync(TableAvailabilityCheckDTO checkDto);
        Task<TableReservationReadDTO> ConfirmReservationAsync(int id);
        Task<TableReservationReadDTO> CheckInReservationAsync(int id);
        Task<TableReservationReadDTO> CompleteReservationAsync(int id);
        Task<TableReservationReadDTO> CancelReservationAsync(int id);
        Task<List<TableReservationReadDTO>> GetTodayReservationsAsync(int branchId);
        Task<List<TableReservationReadDTO>> GetUpcomingReservationsAsync(int branchId, int days = 7);
    }
}