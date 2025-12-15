using System;
using System.Collections.Generic;

namespace BarnManagement.Core.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid AnimalId { get; set; }

    public string ProductType { get; set; } = null!;
    
    public int Quantity { get; set; } = 1;

    public decimal SalePrice { get; set; }

    public DateTime ProducedAt { get; set; }

    public virtual Animal Animal { get; set; } = null!;
}
