using System;
using System.Collections.Generic;
using BarnManagement.Core.Enums;

namespace BarnManagement.Core.Entities;

public partial class Animal
{
    public Guid Id { get; set; }

    public Guid FarmId { get; set; }

    public AnimalSpecies Species { get; set; }

    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public int LifeSpanDays { get; set; }

    public int ProductionInterval { get; set; }

    public DateTime? NextProductionAt { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SellPrice { get; set; }



    public virtual Farm Farm { get; set; } = null!;
}
