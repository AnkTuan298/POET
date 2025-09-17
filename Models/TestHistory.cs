using System;
using System.Collections.Generic;

namespace POET.Models;

public partial class TestHistory
{
    public int HistoryId { get; set; }

    public int UserId { get; set; }

    public string TestName { get; set; } = null!;

    public int Score { get; set; }

    public DateTime DateTaken { get; set; }

    public virtual User User { get; set; } = null!;
}
