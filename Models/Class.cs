using System;
using System.Collections.Generic;

namespace POET.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int TeacherId { get; set; }

    public string ClassCode { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<User> Students { get; set; } = new List<User>();
}
