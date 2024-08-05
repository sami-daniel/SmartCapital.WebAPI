using System;
using System.Collections.Generic;

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class Category
{
    public uint CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime CategoryCreationDate { get; set; }

    public string? CategoryDescription { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
}
