using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class BranchCreateDTO
    {
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }
    }

    public class BranchUpdateDTO
    {

        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }
    }

    public class BranchReadDTO
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
    }

    public class BranchDeleteDTO
    {
        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int Id { get; set; }
    }
}