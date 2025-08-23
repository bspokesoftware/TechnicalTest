using TechnicalTest.Data;

namespace TechnicalTest.API.Models.BankAccounts
{
    public static class BankAccountMappers
    {
        public static BankAccountResponse ToResponse(this BankAccount account) =>
            new(
                account.Id,
                account.AccountNumber,
                account.IsFrozen,
                account.Balance,
                account.CustomerId,
                account.Customer?.Name ?? string.Empty
            );
    }
}
