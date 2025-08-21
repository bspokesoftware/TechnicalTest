namespace TechnicalTest.Data;

public class BankAccount
{
    public int Id { get; set; }
    public required string AccountNumber { get; set; }
    public bool IsFrozen { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Transfer> TransfersOut { get; set; } = new HashSet<Transfer>();
    public ICollection<Transfer> TransfersIn { get; set; } = new HashSet<Transfer>();

}