using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Services
{
    public class TableReservationService : ITableReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITableService _tableService;

        public TableReservationService(ApplicationDbContext context, ITableService tableService)
        {
            _context = context;
            _tableService = tableService;
        }

        public async Task<List<TableReservationReadDTO>> GetAllReservationsAsync()
        {
            return await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Select(r => MapToReadDTO(r))
                .ToListAsync();
        }

        public async Task<List<TableReservationReadDTO>> GetReservationsByBranchAsync(int branchId)
        {
            return await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.BranchId == branchId)
                .Select(r => MapToReadDTO(r))
                .ToListAsync();
        }

        public async Task<List<TableReservationReadDTO>> GetReservationsByCustomerAsync(string customerName, string? customerPhone = null)
        {
            var query = _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.CustomerName.Contains(customerName));

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(r => r.CustomerPhone == customerPhone);
            }

            return await query
                .Select(r => MapToReadDTO(r))
                .OrderByDescending(r => r.ReservationDateTime)
                .ToListAsync();
        }

        public async Task<TableReservationReadDTO> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.Id == id)
                .Select(r => MapToReadDTO(r))
                .FirstOrDefaultAsync();

            if (reservation == null)
                throw new Exception("Reservation not found");

            return reservation;
        }

        public async Task<TableReservationReadDTO> GetReservationByCodeAsync(string reservationCode)
        {
            var reservation = await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.ReservationCode == reservationCode)
                .Select(r => MapToReadDTO(r))
                .FirstOrDefaultAsync();

            if (reservation == null)
                throw new Exception("Reservation not found");

            return reservation;
        }

        public async Task<TableReservationReadDTO> CreateReservationAsync(TableReservationCreateDTO reservationDto)
        {
            // Validate table and branch
            var table = await _context.Tables.FindAsync(reservationDto.TableId);
            if (table == null)
                throw new Exception("Table not found");

            if (table.BranchId != reservationDto.BranchId)
                throw new Exception("Table does not belong to the specified branch");

            if (!table.IsAvailable)
                throw new Exception("Table is not available");

            if (table.Capacity < reservationDto.GuestCount)
                throw new Exception($"Table capacity ({table.Capacity}) is insufficient for guest count ({reservationDto.GuestCount})");

            // Check table availability for the requested time slot
            var endTime = reservationDto.ReservationDateTime.AddHours(reservationDto.DurationHours);
            var conflictingReservation = await _context.TableReservations
                .AnyAsync(r => r.TableId == reservationDto.TableId &&
                             (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn) &&
                             ((reservationDto.ReservationDateTime >= r.ReservationDateTime && reservationDto.ReservationDateTime < r.ReservationDateTime.AddHours(r.DurationHours)) ||
                              (endTime > r.ReservationDateTime && endTime <= r.ReservationDateTime.AddHours(r.DurationHours)) ||
                              (reservationDto.ReservationDateTime <= r.ReservationDateTime && endTime >= r.ReservationDateTime.AddHours(r.DurationHours))));

            if (conflictingReservation)
                throw new Exception("Table is already reserved for the requested time slot");

            // Generate unique reservation code
            var reservationCode = await GenerateReservationCodeAsync();

            var reservation = new TableReservation
            {
                ReservationCode = reservationCode,
                CustomerName = reservationDto.CustomerName,
                CustomerPhone = reservationDto.CustomerPhone,
                CustomerEmail = reservationDto.CustomerEmail,
                TableId = reservationDto.TableId,
                BranchId = reservationDto.BranchId,
                ReservationDateTime = reservationDto.ReservationDateTime,
                DurationHours = reservationDto.DurationHours,
                GuestCount = reservationDto.GuestCount,
                Notes = reservationDto.Notes,
                CreatedBy = reservationDto.CreatedBy,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TableReservations.Add(reservation);
            await _context.SaveChangesAsync();

            return await GetReservationByIdAsync(reservation.Id);
        }

        public async Task<TableReservationReadDTO> UpdateReservationAsync(int id, TableReservationUpdateDTO reservationDto)
        {
            var reservation = await _context.TableReservations.FindAsync(id);
            if (reservation == null)
                throw new Exception("Reservation not found");

            // Check if reservation can be updated
            if (reservation.Status == ReservationStatus.Completed || reservation.Status == ReservationStatus.Cancelled)
                throw new Exception("Cannot update completed or cancelled reservation");

            // If changing time or duration, validate availability
            if (reservation.ReservationDateTime != reservationDto.ReservationDateTime || 
                reservation.DurationHours != reservationDto.DurationHours)
            {
                var endTime = reservationDto.ReservationDateTime.AddHours(reservationDto.DurationHours);
                var conflictingReservation = await _context.TableReservations
                    .AnyAsync(r => r.TableId == reservation.TableId &&
                                 r.Id != id &&
                                 (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn) &&
                                 ((reservationDto.ReservationDateTime >= r.ReservationDateTime && reservationDto.ReservationDateTime < r.ReservationDateTime.AddHours(r.DurationHours)) ||
                                  (endTime > r.ReservationDateTime && endTime <= r.ReservationDateTime.AddHours(r.DurationHours)) ||
                                  (reservationDto.ReservationDateTime <= r.ReservationDateTime && endTime >= r.ReservationDateTime.AddHours(r.DurationHours))));

                if (conflictingReservation)
                    throw new Exception("Table is already reserved for the requested time slot");
            }

            // Validate guest count against table capacity
            var table = await _context.Tables.FindAsync(reservation.TableId);
            if (table != null && table.Capacity < reservationDto.GuestCount)
                throw new Exception($"Table capacity ({table.Capacity}) is insufficient for guest count ({reservationDto.GuestCount})");

            reservation.CustomerName = reservationDto.CustomerName;
            reservation.CustomerPhone = reservationDto.CustomerPhone;
            reservation.CustomerEmail = reservationDto.CustomerEmail;
            reservation.ReservationDateTime = reservationDto.ReservationDateTime;
            reservation.DurationHours = reservationDto.DurationHours;
            reservation.GuestCount = reservationDto.GuestCount;
            reservation.Status = reservationDto.Status;
            reservation.Notes = reservationDto.Notes;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetReservationByIdAsync(reservation.Id);
        }

        public async Task<bool> DeleteReservationAsync(TableReservationDeleteDTO reservationDto)
        {
            var reservation = await _context.TableReservations.FindAsync(reservationDto.Id);
            if (reservation == null)
                return false;

            if (reservation.Status == ReservationStatus.CheckedIn)
                throw new Exception("Cannot delete checked-in reservation");

            _context.TableReservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TableReadDTO>> CheckTableAvailabilityAsync(TableAvailabilityCheckDTO checkDto)
        {
            return await _tableService.GetAvailableTablesAsync(checkDto);
        }

        public async Task<TableReservationReadDTO> ConfirmReservationAsync(int id)
        {
            return await UpdateReservationStatusAsync(id, ReservationStatus.Confirmed);
        }

        public async Task<TableReservationReadDTO> CheckInReservationAsync(int id)
        {
            return await UpdateReservationStatusAsync(id, ReservationStatus.CheckedIn);
        }

        public async Task<TableReservationReadDTO> CompleteReservationAsync(int id)
        {
            return await UpdateReservationStatusAsync(id, ReservationStatus.Completed);
        }

        public async Task<TableReservationReadDTO> CancelReservationAsync(int id)
        {
            return await UpdateReservationStatusAsync(id, ReservationStatus.Cancelled);
        }

        public async Task<List<TableReservationReadDTO>> GetTodayReservationsAsync(int branchId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.BranchId == branchId && 
                           r.ReservationDateTime >= today && 
                           r.ReservationDateTime < tomorrow)
                .Select(r => MapToReadDTO(r))
                .ToListAsync();
        }

        public async Task<List<TableReservationReadDTO>> GetUpcomingReservationsAsync(int branchId, int days = 7)
        {
            var today = DateTime.Today;
            var futureDate = today.AddDays(days);

            return await _context.TableReservations
                .Include(r => r.Table)
                .Include(r => r.Branch)
                .Where(r => r.BranchId == branchId && 
                           r.ReservationDateTime >= today && 
                           r.ReservationDateTime < futureDate &&
                           (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending))
                .Select(r => MapToReadDTO(r))
                .ToListAsync();
        }

        private async Task<TableReservationReadDTO> UpdateReservationStatusAsync(int id, ReservationStatus status)
        {
            var reservation = await _context.TableReservations.FindAsync(id);
            if (reservation == null)
                throw new Exception("Reservation not found");

            // Validate status transitions
            if (!IsValidStatusTransition(reservation.Status, status))
                throw new Exception($"Cannot change status from {reservation.Status} to {status}");

            reservation.Status = status;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetReservationByIdAsync(reservation.Id);
        }

        private bool IsValidStatusTransition(ReservationStatus currentStatus, ReservationStatus newStatus)
        {
            return currentStatus switch
            {
                ReservationStatus.Pending => newStatus == ReservationStatus.Confirmed || newStatus == ReservationStatus.Cancelled,
                ReservationStatus.Confirmed => newStatus == ReservationStatus.CheckedIn || newStatus == ReservationStatus.Cancelled,
                ReservationStatus.CheckedIn => newStatus == ReservationStatus.Completed,
                ReservationStatus.Completed => false,
                ReservationStatus.Cancelled => false,
                _ => false
            };
        }

        private async Task<string> GenerateReservationCodeAsync()
        {
            string code;
            bool exists;
            
            do
            {
                code = $"RSV{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
                exists = await _context.TableReservations.AnyAsync(r => r.ReservationCode == code);
            } while (exists);

            return code;
        }

        private static TableReservationReadDTO MapToReadDTO(TableReservation reservation)
        {
            return new TableReservationReadDTO
            {
                Id = reservation.Id,
                ReservationCode = reservation.ReservationCode,
                CustomerName = reservation.CustomerName,
                CustomerPhone = reservation.CustomerPhone,
                CustomerEmail = reservation.CustomerEmail,
                TableId = reservation.TableId,
                TableNumber = reservation.Table?.TableNumber ?? "",
                BranchId = reservation.BranchId,
                BranchName = reservation.Branch?.BranchName ?? "",
                ReservationDateTime = reservation.ReservationDateTime,
                DurationHours = reservation.DurationHours,
                ReservationEndTime = reservation.ReservationEndTime,
                GuestCount = reservation.GuestCount,
                Status = reservation.Status,
                Notes = reservation.Notes,
                CreatedBy = reservation.CreatedBy,
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt,
                IsActive = reservation.IsActive
            };
        }
    }
}