using System;

namespace backendcafe.Models{
      public class Transaction
      {
            public int Id { get; set; }
            public string Name { get; set; }
            public string TransactionCode { get; set; }
            public int Total { get; set; }
            public string Status { get; set; }
            public int BranchId { get; set; }
            public Branch Branch { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
      }
}