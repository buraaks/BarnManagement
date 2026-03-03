using System;
using System.Collections.Generic;

namespace BarnManagement.Core.Entities;
// Kısa mimari özet: Bu entity, sistem kullanıcısının kimlik, yetkiye temel olan ve finansal alanlarını temsil eder.


public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public decimal Balance { get; set; }

    public virtual ICollection<Farm> Farms { get; set; } = new List<Farm>();
}
