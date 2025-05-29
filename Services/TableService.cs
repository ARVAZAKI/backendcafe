using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using Microsoft.EntityFrameworkCore;

namespace backendcafe.Services
{
    public class TableService : ITableService
    {
        private readonly ApplicationDbContext _context;

        public TableService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TableReadDTO>> GetAllTablesAsync()
        {
            return await _context.Tables
                .Include(t => t.Branch)
                .Select(t => new TableReadDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsAvailable = t.IsAvailable,
                    BranchId = t.BranchId,
                    BranchName = t.Branch!.BranchName,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<TableReadDTO>> GetTablesByBranchAsync(int branchId)
        {
            return await _context.Tables
                .Include(t => t.Branch)
                .Where(t => t.BranchId == branchId)
                .Select(t => new TableReadDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsAvailable = t.IsAvailable,
                    BranchId = t.BranchId,
                    BranchName = t.Branch!.BranchName,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<TableReadDTO> GetTableByIdAsync(int id)
        {
            var table = await _context.Tables
                .Include(t => t.Branch)
                .Where(t => t.Id == id)
                .Select(t => new TableReadDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsAvailable = t.IsAvailable,
                    BranchId = t.BranchId,
                    BranchName = t.Branch!.BranchName,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (table == null)
                throw new Exception("Table not found");

            return table;
        }

        public async Task<TableReadDTO> CreateTableAsync(TableCreateDTO tableDto)
        {
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == tableDto.BranchId);
            if (!branchExists)
                throw new Exception("Branch not found");

            var tableExists = await _context.Tables
                .AnyAsync(t => t.TableNumber == tableDto.TableNumber && t.BranchId == tableDto.BranchId);
            if (tableExists)
                throw new Exception("Table number already exists in this branch");

            var table = new Table
            {
                TableNumber = tableDto.TableNumber,
                Capacity = tableDto.Capacity,
                BranchId = tableDto.BranchId,
                Description = tableDto.Description,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            return await GetTableByIdAsync(table.Id);
        }

        public async Task<TableReadDTO> UpdateTableAsync(int id, TableUpdateDTO tableDto)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                throw new Exception("Table not found");

            var tableExists = await _context.Tables
                .AnyAsync(t => t.TableNumber == tableDto.TableNumber && 
                             t.BranchId == table.BranchId && 
                             t.Id != id);
            if (tableExists)
                throw new Exception("Table number already exists in this branch");

            table.TableNumber = tableDto.TableNumber;
            table.Capacity = tableDto.Capacity;
            table.IsAvailable = tableDto.IsAvailable;
            table.Description = tableDto.Description;
            table.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetTableByIdAsync(table.Id);
        }

        public async Task<bool> DeleteTableAsync(TableDeleteDTO tableDto)
        {
            var table = await _context.Tables.FindAsync(tableDto.Id);
            if (table == null)
                return false;

            var hasActiveReservations = await _context.TableReservations
                .AnyAsync(r => r.TableId == tableDto.Id && 
                             (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn));
            
            if (hasActiveReservations)
                throw new Exception("Cannot delete table with active reservations");

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TableReadDTO>> GetAvailableTablesAsync(TableAvailabilityCheckDTO checkDto)
        {
            var endTime = checkDto.ReservationDateTime.AddHours(checkDto.DurationHours);

            var availableTables = await _context.Tables
                .Include(t => t.Branch)
                .Where(t => t.BranchId == checkDto.BranchId && 
                           t.IsAvailable && 
                           t.Capacity >= checkDto.GuestCount &&
                           !_context.TableReservations.Any(r => 
                               r.TableId == t.Id &&
                               (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.CheckedIn) &&
                               ((checkDto.ReservationDateTime >= r.ReservationDateTime && checkDto.ReservationDateTime < r.ReservationDateTime.AddHours(r.DurationHours)) ||
                                (endTime > r.ReservationDateTime && endTime <= r.ReservationDateTime.AddHours(r.DurationHours)) ||
                                (checkDto.ReservationDateTime <= r.ReservationDateTime && endTime >= r.ReservationDateTime.AddHours(r.DurationHours)))))
                .Select(t => new TableReadDTO
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsAvailable = t.IsAvailable,
                    BranchId = t.BranchId,
                    BranchName = t.Branch!.BranchName,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return availableTables;
        }
    }
}