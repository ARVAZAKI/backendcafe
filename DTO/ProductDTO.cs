using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public int Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "IsActive status is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }
    }

    public class ProductUpdateDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public int Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "IsActive status is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive number")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }
    }

    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public int BranchId { get; set; }
    }
}
