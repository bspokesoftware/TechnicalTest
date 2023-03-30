namespace TechnicalTest.Data;

public class BankAccount
{
    public int Id { get; set; }
    public required string AccountNumber { get; set; }
    
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}