using System;
using System.Collections.Generic;

namespace BarnManagement.Core.Entities;

public partial class Animal
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public string Species { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public int LifeSpanDays { get; set; }

    public int ProductionInterval { get; set; }

    public DateTime? NextProductionAt { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SellPrice { get; set; }

    public bool IsSold { get; set; }

    public virtual Farm Farm { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
