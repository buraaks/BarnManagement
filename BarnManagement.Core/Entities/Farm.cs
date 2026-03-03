using System;
using System.Collections.Generic;

namespace BarnManagement.Core.Entities;
// Kısa mimari özet: Bu entity, çiftlik domainini ve sahiplik/ilişki bilgilerini veri modeli olarak temsil eder.


public partial class Farm
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User Owner { get; set; } = null!;
}
