namespace TechnicalTest.Data
{
    public class Transfer
    {
        public int Id { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        // Using DateTimeOffset for time to be unambiguous
        public DateTimeOffset CreatedAtUTC { get; set; }
        public BankAccount? FromAccount {  get; set; }
        public BankAccount? ToAccount { get; set; }


    }
}
