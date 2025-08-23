using Microsoft.EntityFrameworkCore;

namespace TechnicalTest.Data.Services.Transfers
{
    public sealed class TransferService : ITransferService
    {
        private readonly ApplicationContext _dbContext;
        public TransferService(ApplicationContext context) => _dbContext = context;

        public async Task<Transfer> CreateAsync( int fromAccountId, int toAccountId, decimal amount, string? reference, CancellationToken cancellationToken = default)
        {
            if(fromAccountId <=  0 || toAccountId <= 0)
            {
                throw TransferException.ValidationFailed("Account ids must be provided");
            }
            if (fromAccountId == toAccountId)
            {
                throw TransferException.ValidationFailed("Cannot transfer to the same account");
            }
            if (amount <= 0)
            {
                throw TransferException.ValidationFailed("Transfer amount must be greater than zero");
            }

            var accounts = await _dbContext.BankAccounts
                .Where(account => account.Id == fromAccountId || account.Id == toAccountId)
                .Select(account => new { account.Id, account.CustomerId, account.IsFrozen, account.Balance})
                .ToListAsync(cancellationToken);

            var from = accounts.FirstOrDefault(a => a.Id == fromAccountId);
            var to = accounts.FirstOrDefault(a => a.Id == toAccountId);

            if (from is null || to is null)
            {
                throw TransferException.NotFound("One or both Accounts were not found");
            }
            if (from.CustomerId != to.CustomerId)
            {
                throw TransferException.BusinessRuleViolation("Transfers are only available to accounts that the user owns");
            }
            if (from.IsFrozen)
            {
                throw TransferException.BusinessRuleViolation("Origin account is frozen");
            }
            if (to.IsFrozen)
            {
                throw TransferException.BusinessRuleViolation("Receving account is frozen");
            }

            var customer = await _dbContext.Customers
                .Where(c => c.Id == from.CustomerId)
                .Select(c => new {c.Id, c.DailyTransferLimit})
                .SingleOrDefaultAsync(cancellationToken);

            if (customer is null)
            {
                throw TransferException.NotFound("Please recheck the details. No customer found");
            }

            DateTime startOfTodayUtc = DateTimeOffset.UtcNow.Date;
            DateTime startOfTommorrowUtc = startOfTodayUtc.AddDays(1);

            Decimal todaysTotal = await _dbContext.Transfers.Where(t =>
            t.CreatedAtUTC >= startOfTodayUtc &&
            t.CreatedAtUTC < startOfTommorrowUtc &&
            t.FromAccountId == fromAccountId)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            var fromAccount = await _dbContext.BankAccounts.FirstAsync(account => account.Id == fromAccountId, cancellationToken);
            var toAccount = await _dbContext.BankAccounts.FirstAsync(account => account.Id == toAccountId, cancellationToken);

            fromAccount.Balance -= amount;
            toAccount.Balance -= amount;

            Transfer transfer = new Transfer
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = decimal.Round(amount, 2, MidpointRounding.ToZero),
                Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim(),
                CreatedAtUTC = DateTimeOffset.UtcNow
            };

            _dbContext.Transfers.Add(transfer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return transfer;
        }

        public async Task<IReadOnlyList<Transfer>> GetByAccountAsync(int accountId, DateTimeOffset? fromUTC = null, DateTimeOffset? toUTC = null, CancellationToken cancellationToken = default)
        {
            if (accountId <= 0)
            {
                throw TransferException.ValidationFailed("Account id must be positive.");
            }

            // Ensure account exist
            bool exists = await _dbContext.BankAccounts
                .AnyAsync(a => a.Id == accountId, cancellationToken);

            if (!exists)
            {
                throw TransferException.NotFound("Bank account not found.");
            }

            var query = _dbContext.Transfers.AsQueryable()
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId);

            if (fromUTC.HasValue)
                query = query.Where(t => t.CreatedAtUTC >= fromUTC.Value);

            if (toUTC.HasValue)
                query = query.Where(t => t.CreatedAtUTC < toUTC.Value);

            return await query
                .OrderByDescending(t => t.CreatedAtUTC)
                .ToListAsync(cancellationToken);
        }
    }
}
