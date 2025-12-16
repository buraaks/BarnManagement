using System;
using System.Collections.Generic;

namespace BarnManagement.Core.Entities;

public partial class Farm
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OwnerId { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User Owner { get; set; } = null!;
}
