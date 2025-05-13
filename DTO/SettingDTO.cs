using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class SettingCreateDTO
    {
        [Required(ErrorMessage = "openingTime ID is required")]
        public TimeOnly OpeningTime { get; set; }

        [Required(ErrorMessage = "closingTime ID is required")]
        public TimeOnly ClosingTime { get; set;}

        [Required(ErrorMessage = "WifiPassword ID is required")]
        public string WifiPassword { get; set; }

        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive number")]
        public int BranchId { get; set; }
    }

    public class SettingUpdateDTO
    {
        public TimeOnly OpeningTime { get; set; }
        
        public TimeOnly ClosingTime { get; set; }
        public string WifiPassword { get; set; }
       
    }

    public class SettingReadDTO
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public TimeOnly OpeningTime { get; set; }       
        public TimeOnly ClosingTime { get; set;}
        public string WifiPassword { get; set; }

        
    }

}