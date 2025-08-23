namespace TechnicalTest.API.Models.BankAccounts
{
    public record BankAccountResponse(
        int Id,
        string AccountNumber,
        bool IsFrozen,
        decimal Balance,
        int CustomerId,
        string CustomerName
    );
}
