using System;
using System.Collections.Generic;

namespace BarnManagement.DataAccess.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid AnimalId { get; set; }

    public string ProductType { get; set; } = null!;

    public decimal SalePrice { get; set; }

    public DateTime ProducedAt { get; set; }

    public virtual Animal Animal { get; set; } = null!;
}
