using System.ComponentModel.DataAnnotations;

namespace backendcafe.Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        Completed,
        Cancelled
    }

    public class TableReservation
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ReservationCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(15)]
        public string? CustomerPhone { get; set; }
        
        [StringLength(100)]
        public string? CustomerEmail { get; set; }
        
        [Required]
        public int TableId { get; set; }
        public Table? Table { get; set; }
        
        [Required]
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        
        [Required]
        public DateTime ReservationDateTime { get; set; }
        
        [Required]
        [Range(1, 8)]
        public int DurationHours { get; set; } = 1; 
        
        [Required]
        [Range(1, 20)]
        public int GuestCount { get; set; }
        
        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ReservationEndTime => ReservationDateTime.AddHours(DurationHours);
        public bool IsActive => Status == ReservationStatus.Confirmed || Status == ReservationStatus.CheckedIn;
    }
}