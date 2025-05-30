using System.ComponentModel.DataAnnotations;
using backendcafe.Models;

namespace backendcafe.DTO
{
    public class TableReservationCreateDTO
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Customer phone cannot exceed 15 characters")]
        public string? CustomerPhone { get; set; }

        [StringLength(100, ErrorMessage = "Customer email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? CustomerEmail { get; set; }

        [Required(ErrorMessage = "Table ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Table ID must be a positive number")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Reservation date time is required")]
        public DateTime ReservationDateTime { get; set; }

        [Required(ErrorMessage = "Duration hours is required")]
        [Range(1, 8, ErrorMessage = "Duration must be between 1 and 8 hours")]
        public int DurationHours { get; set; } = 2;

        [Required(ErrorMessage = "Guest count is required")]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20")]
        public int GuestCount { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Created by is required")]
        [StringLength(100, ErrorMessage = "Created by cannot exceed 100 characters")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class TableReservationUpdateDTO
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "Customer phone cannot exceed 15 characters")]
        public string? CustomerPhone { get; set; }

        [StringLength(100, ErrorMessage = "Customer email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? CustomerEmail { get; set; }

        [Required(ErrorMessage = "Reservation date time is required")]
        public DateTime ReservationDateTime { get; set; }

        [Required(ErrorMessage = "Duration hours is required")]
        [Range(1, 8, ErrorMessage = "Duration must be between 1 and 8 hours")]
        public int DurationHours { get; set; }

        [Required(ErrorMessage = "Guest count is required")]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20")]
        public int GuestCount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ReservationStatus Status { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }

    public class TableReservationReadDTO
    {
        public int Id { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime ReservationDateTime { get; set; }
        public int DurationHours { get; set; }
        public DateTime ReservationEndTime { get; set; }
        public int GuestCount { get; set; }
        public ReservationStatus Status { get; set; }
        public string? Notes { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class TableReservationDeleteDTO
    {
        [Required(ErrorMessage = "Reservation ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Reservation ID must be a positive number")]
        public int Id { get; set; }
    }

    public class TableAvailabilityCheckDTO
    {
        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Reservation date time is required")]
        public DateTime ReservationDateTime { get; set; }

        [Required(ErrorMessage = "Duration hours is required")]
        [Range(1, 8, ErrorMessage = "Duration must be between 1 and 8 hours")]
        public int DurationHours { get; set; }

        [Required(ErrorMessage = "Guest count is required")]
        [Range(1, 20, ErrorMessage = "Guest count must be between 1 and 20")]
        public int GuestCount { get; set; }
    }
}