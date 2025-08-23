namespace TechnicalTest.API.Models.BankAccounts
{
    public sealed record CreateBankAccountRequest(int CustomerId, string AccountNumber);
}
