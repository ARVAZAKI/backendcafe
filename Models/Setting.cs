using System;
namespace backendcafe.Models;

public class Setting{
      public int Id { get; set; }
      public int BranchId { get; set; } 
      public Branch Branch { get; set; }

      public TimeOnly OpeningTime { get; set; } = new TimeOnly();
      public TimeOnly ClosingTime { get; set; } = new TimeOnly();

      public string WifiPassword { get; set; } = string.Empty;
}