using System;
using System.Collections.Generic;

namespace POET.Models;

public partial class Profile
{
    public int UserId { get; set; }

    public string Name { get; set; } = "Anonymous";

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int TestTaken { get; set; }

    public virtual User User { get; set; } = null!;
}
