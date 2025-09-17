using System;
using System.Collections.Generic;

namespace POET.Models;

public partial class Level
{
    public int LevelId { get; set; }

    public string LevelName { get; set; } = null!;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
