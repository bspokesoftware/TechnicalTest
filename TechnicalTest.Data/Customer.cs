namespace TechnicalTest.Data;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
}