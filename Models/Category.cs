using System;

namespace backendcafe.Models;

public class Category
{

    public int Id { get; set; }
    public string CategoryName { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; }


}
