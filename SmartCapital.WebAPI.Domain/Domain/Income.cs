using System;
using System.Collections.Generic;

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class Income
{
    public uint IncomeId { get; set; }

    public DateOnly IncomeDate { get; set; }

    public string IncomeTitle { get; set; } = null!;

    public decimal IncomeAmount { get; set; }

    public DateTime IncomeCreationDate { get; set; }

    public string? IncomeDescription { get; set; }

    public uint ProfileId { get; set; }

    public uint? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Profile Profile { get; set; } = null!;
}
