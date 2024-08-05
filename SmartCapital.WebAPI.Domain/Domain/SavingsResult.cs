using System;
using System.Collections.Generic;

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class SavingsResult
{
    public uint ProfileId { get; set; }

    public string ProfileName { get; set; } = null!;

    public decimal? TotalIncome { get; set; }

    public decimal? TotalExpense { get; set; }

    public decimal? TotalEconomy { get; set; }
}
