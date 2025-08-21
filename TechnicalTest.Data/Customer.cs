namespace TechnicalTest.Data;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public decimal DailyTransferLimit { get; set; }

    // Collection of bank accounts owned by a customer
    public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
}