// none

namespace SmartCapital.WebAPI.Domain.Domain;

public partial class Profile
{
    public uint ProfileId { get; set; }

    public DateTime ProfileCreationDate { get; set; }

    public string ProfileName { get; set; } = null!;

    public decimal? ProfileOpeningBalance { get; set; }

    public uint UsersUserId { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    public virtual User UsersUser { get; set; } = null!;
}
