using Microsoft.EntityFrameworkCore;

namespace TechnicalTest.Data;

public class ApplicationContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=bin\\database.db;");
        base.OnConfiguring(optionsBuilder);
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    public DbSet<Transfer> Transfers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(c => c.Name).IsRequired();
            e.Property(c => c.DailyTransferLimit).HasPrecision(18, 2);
        });

        //Bank Account
        modelBuilder.Entity<BankAccount>(e =>
        {
            e.Property(a => a.AccountNumber).IsRequired();
            e.HasIndex(a => a.AccountNumber).IsUnique();
            e.HasMany(a => a.TransfersOut).WithOne(t => t.FromAccount)
                .HasForeignKey(t => t.FromAccountId).OnDelete(DeleteBehavior.Restrict);

            e.HasMany(a => a.TransfersIn).WithOne(t => t.ToAccount)
                .HasForeignKey(t => t.ToAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        //Transer
        modelBuilder.Entity<Transfer>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Reference).HasMaxLength(240);
            e.HasIndex(t => t.CreatedAtUTC);
            e.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Transfer_Amount_Positive", "[Amount] > 0");
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}