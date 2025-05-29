using System.ComponentModel.DataAnnotations;

namespace backendcafe.Models
{
    public class Table
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string TableNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 20)]
        public int Capacity { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; } = true;
        
        [Required]
        public int BranchId { get; set; }
        
        public Branch? Branch { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}