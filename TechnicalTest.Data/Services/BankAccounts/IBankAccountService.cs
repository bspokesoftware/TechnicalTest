namespace TechnicalTest.Data.Services.BankAccounts
{
    public interface IBankAccountService
    {
        Task<BankAccount> CreateAsync(int customerId, string accountNumber, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BankAccount>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<BankAccount> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<BankAccount> UpdateFrozenAsync(int id, bool isFrozen, CancellationToken cancellationToken = default);

        Task<BankAccount> UpdateBalanceAsync(int id, decimal amount, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
