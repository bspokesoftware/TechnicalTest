using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalTest.Data.Services
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
