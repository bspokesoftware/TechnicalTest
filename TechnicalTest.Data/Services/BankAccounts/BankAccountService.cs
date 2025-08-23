using Microsoft.EntityFrameworkCore;

namespace TechnicalTest.Data.Services.BankAccounts
{
    public class BankAccountService : IBankAccountService
    {
        private readonly ApplicationContext _dbContext;

        public BankAccountService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BankAccount> CreateAsync(int customerId, string accountNumber, CancellationToken cancellationToken = default)
        {
            if (customerId <= 0)
            {
                throw BankAccountException.ValidationFailed("Customer id must be provided");
            }
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw BankAccountException.ValidationFailed("Account number is required");
            }

            bool customerExists = await _dbContext.Customers.AnyAsync(c => c.Id == customerId, cancellationToken);
            if (!customerExists)
            {
                throw BankAccountException.NotFound($"Customer {customerId} not found");
            }

            bool duplicate = await _dbContext.BankAccounts.AnyAsync(a => a.AccountNumber == accountNumber, cancellationToken);
            if (duplicate)
            {
                throw BankAccountException.Conflict("Account number already exists");
            }

            BankAccount account = new BankAccount
            {
                CustomerId = customerId,
                AccountNumber = accountNumber.Trim(),
                IsFrozen = false
            };

            _dbContext.BankAccounts.Add(account);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw BankAccountException.Conflict("Failed to create account due to a unique constraint violation");
            }

            await _dbContext.Entry(account).Reference(a => a.Customer!).LoadAsync(cancellationToken);

            return account;
        }

        public async Task<IReadOnlyList<BankAccount>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.BankAccounts
                .Include(a => a.Customer)
                .OrderBy(a => a.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<BankAccount> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            BankAccount account = await _dbContext.BankAccounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            return account ?? throw BankAccountException.NotFound($"Bank account {id} not found");
        }

        public async Task<BankAccount> UpdateFrozenAsync(int id, bool isFrozen, CancellationToken cancellationToken = default)
        {
            BankAccount account = await _dbContext.BankAccounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (account is null) throw BankAccountException.NotFound($"Bank account {id} not found");

            account.IsFrozen = isFrozen;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return account;
        }

        public async Task<BankAccount> UpdateBalanceAsync(int accountId, decimal amount,CancellationToken cancellationToken = default)
        {
            if(amount == 0)
            {
                throw new ArgumentException("Amount muist not be zero", nameof(amount));
            }

            var account = await _dbContext.BankAccounts
                .SingleOrDefaultAsync(account => account.Id == accountId, cancellationToken);

            if(account is null)
            {
                throw new InvalidOperationException("Bank account not found");
            }
            if(account.IsFrozen)
            {
                throw new InvalidOperationException("Cannot modufy balance of a frozen account ");
            }
            if(account.Balance + amount< 0)
            {
                throw new InvalidOperationException("Inssufficient funds");
            }

            account.Balance += amount;

            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return account;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            BankAccount account = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (account is null) throw BankAccountException.NotFound($"Bank account {id} not found");

            // Prevent deleting accounts that have transfer history (matches DeleteBehavior.Restrict intent)
            bool hasTransfers = await _dbContext.Transfers.AnyAsync(
                t => t.FromAccountId == id || t.ToAccountId == id, cancellationToken);

            if (hasTransfers)
                throw BankAccountException.Conflict("Cannot delete an account with existing transfer history");

            _dbContext.BankAccounts.Remove(account);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw BankAccountException.Conflict("Delete failed due to related data.");
            }
        }
    }
}
