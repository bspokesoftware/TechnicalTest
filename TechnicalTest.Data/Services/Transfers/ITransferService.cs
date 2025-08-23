namespace TechnicalTest.Data.Services.Transfers
{
    public interface ITransferService
    {
        Task<Transfer> CreateAsync(
            int fromAccountId,
            int toAccountId,
            decimal amount,
            string? reference,
            CancellationToken cancellationToken = default
            );

        Task<IReadOnlyList<Transfer>> GetByAccountAsync(
            int accountId,
            DateTimeOffset? fromUtc = null,
            DateTimeOffset? toUtc = null,
            CancellationToken cancellationToken = default
            );
    }
}
