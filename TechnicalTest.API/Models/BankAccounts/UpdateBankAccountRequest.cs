namespace TechnicalTest.API.Models.BankAccounts
{
    public record UpdateBankAccountRequest(bool IsFrozen);
    public record UpdateBalanceRequest(decimal Amount);
}
