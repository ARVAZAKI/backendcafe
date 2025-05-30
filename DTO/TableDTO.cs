using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class TableCreateDTO
    {
        [Required(ErrorMessage = "Table number is required")]
        [StringLength(20, ErrorMessage = "Table number cannot exceed 20 characters")]
        public string TableNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    public class TableUpdateDTO
    {
        [Required(ErrorMessage = "Table number is required")]
        [StringLength(20, ErrorMessage = "Table number cannot exceed 20 characters")]
        public string TableNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "IsAvailable is required")]
        public bool IsAvailable { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    public class TableReadDTO
    {
        public int Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TableDeleteDTO
    {
        [Required(ErrorMessage = "Table ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Table ID must be a positive number")]
        public int Id { get; set; }
    }
}