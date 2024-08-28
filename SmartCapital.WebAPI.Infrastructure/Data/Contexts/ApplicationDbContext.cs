using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Infrastructure.Data.Contexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Income> Incomes { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<SavingsResult> SavingsResults { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.CategoryName, "CategoryName_UNIQUE").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryCreationDate).HasColumnType("datetime");
            entity.Property(e => e.CategoryDescription).HasColumnType("mediumtext");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => new { e.ExpenseId, e.ProfileId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("expenses");

            entity.HasIndex(e => e.CategoryId, "fk_Expenses_Category1_idx");

            entity.HasIndex(e => e.ProfileId, "fk_Expenses_Profiles1_idx");

            entity.Property(e => e.ExpenseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ExpenseID");
            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ExpenseAmount).HasPrecision(12, 2);
            entity.Property(e => e.ExpenseCreationDate).HasColumnType("datetime");
            entity.Property(e => e.ExpenseDescription).HasColumnType("mediumtext");
            entity.Property(e => e.ExpenseTitle).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_Expenses_Category");

            entity.HasOne(d => d.Profile).WithMany(p => p.Expenses)
                .HasPrincipalKey(p => p.ProfileId)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("fk_Expenses_Profiles");
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => new { e.IncomeId, e.ProfileId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("incomes");

            entity.HasIndex(e => e.CategoryId, "fk_Incomes_Category1_idx");

            entity.HasIndex(e => e.ProfileId, "fk_Incomes_Profiles_idx");

            entity.Property(e => e.IncomeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("IncomeID");
            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.IncomeAmount).HasPrecision(12, 2);
            entity.Property(e => e.IncomeCreationDate).HasColumnType("datetime");
            entity.Property(e => e.IncomeDescription).HasColumnType("mediumtext");
            entity.Property(e => e.IncomeTitle).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Incomes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_Incomes_Category");

            entity.HasOne(d => d.Profile).WithMany(p => p.Incomes)
                .HasPrincipalKey(p => p.ProfileId)
                .HasForeignKey(d => d.ProfileId)
                .HasConstraintName("fk_Incomes_Profiles");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => new { e.ProfileId, e.UsersUserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("profiles");

            entity.HasIndex(e => e.ProfileId, "ProfileID_UNIQUE").IsUnique();

            entity.HasIndex(e => e.UsersUserId, "fk_Profiles_Users1_idx");

            entity.Property(e => e.ProfileId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProfileID");
            entity.Property(e => e.UsersUserId).HasColumnName("Users_UserID");
            entity.Property(e => e.ProfileCreationDate).HasColumnType("datetime");
            entity.Property(e => e.ProfileName).HasMaxLength(255);
            entity.Property(e => e.ProfileOpeningBalance).HasPrecision(12, 2);

            entity.HasOne(d => d.UsersUser).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.UsersUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Profiles_Users1");
        });

        modelBuilder.Entity<SavingsResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("savings_result");

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.ProfileName).HasMaxLength(255);
            entity.Property(e => e.TotalEconomy).HasPrecision(35, 2);
            entity.Property(e => e.TotalExpense).HasPrecision(34, 2);
            entity.Property(e => e.TotalIncome).HasPrecision(34, 2);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.UserName, "UserName_UNIQUE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserCreationDate).HasColumnType("datetime");
            entity.Property(e => e.UserPassword).HasMaxLength(60);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
