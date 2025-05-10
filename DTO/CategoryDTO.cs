using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }
    }

    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }
    }

    public class CategoryReadDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int BranchId { get; set; }
    }

    public class CategoryDeleteDTO
    {
        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int Id { get; set; }
    }
}