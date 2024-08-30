// none

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class Expense
{
    public uint ExpenseId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public string ExpenseTitle { get; set; } = null!;

    public decimal ExpenseAmount { get; set; }

    public DateTime ExpenseCreationDate { get; set; }

    public string? ExpenseDescription { get; set; }

    public uint ProfileId { get; set; }

    public uint? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Profile Profile { get; set; } = null!;
}
