using System;
using System.Collections.Generic;

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class User
{
    public uint UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public DateTime UserCreationDate { get; set; }

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
