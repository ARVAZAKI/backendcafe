using System.ComponentModel.DataAnnotations;

namespace backendcafe.DTO
{
    public class SettingCreateDTO
    {
        public TimeOnly OpeningTime { get; set; }
        
        public TimeOnly ClosingTime { get; set;}
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
        public TimeOnly OpeningTime { get; set; }
        
        public TimeOnly ClosingTime { get; set;}
        public string WifiPassword { get; set; }

        
    }

}