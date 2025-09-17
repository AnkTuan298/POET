using System;
using System.Collections.Generic;

namespace POET.Models;

public partial class Test
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public int LevelId { get; set; }

    public int? ClassId { get; set; }

    public int CreatedBy { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Level Level { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
