using System;

namespace backendcafe.Models;

public class Cart
{

    public int Id { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; } = 0;
    public int TransactionId { get; set; }
    public virtual Transaction Transaction { get; set; }


}
