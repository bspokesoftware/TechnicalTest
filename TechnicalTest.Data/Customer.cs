namespace TechnicalTest.Data;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal DailyTransferLimit { get; set; }
    public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
}