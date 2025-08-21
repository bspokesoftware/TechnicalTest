using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalTest.Data.Services
{
    public sealed class TransferService : ITransferService
    {
        private readonly ApplicationContext _context;

        public TransferService(ApplicationContext context) => _context = context;

        public async Task<Transfer> CreateAsync(
            int fromAccountId,
            int toAccountId,
            decimal amount,
            string? reference,
            CancellationToken cancellationToken = default)

        {
            if(fromAccountId <=  0 || toAccountId <= 0)
                throw TransferException.ValidationFailed("Account ids must be provided");
            if (fromAccountId == toAccountId)
                throw TransferException.ValidationFailed("Cannot transfer to the same account");
            if (amount <= 0)
                throw TransferException.ValidationFailed("Transfer amount must be greater than zero");

            var accounts = await _context.BankAccounts
                .Where(a => a.Id == fromAccountId || a.Id == toAccountId)
                .Select(a => new { a.Id, a.CustomerId, a.IsFrozen})
                .ToListAsync(cancellationToken);

            var from = accounts.FirstOrDefault(a => a.Id == fromAccountId);
            var to = accounts.FirstOrDefault(a => a.Id == toAccountId);

            if (from is null || to is null)
                throw TransferException.NotFound("One or both Accounts were not found");

            if (from.CustomerId != to.CustomerId)
                throw TransferException.BusinessRuleViolation("Transfers are only available to accounts that the user owns");

            //Checking if Account is frozen
            if (from.IsFrozen)
                throw TransferException.BusinessRuleViolation("Origin account is frozen");
            if (to.IsFrozen)
                throw TransferException.BusinessRuleViolation("Receving account is frozen");

            var customer = await _context.Customers
                .Where(c => c.Id == from.CustomerId)
                .Select(c => new {c.Id, c.DailyTransferLimit})
                .SingleOrDefaultAsync(cancellationToken);

            if (customer == null)
                throw TransferException.NotFound("Please recheck the details. No customer found");

            // Daily limit
            var startOfTodayUtc = DateTimeOffset.UtcNow.Date;
            var startOfTommorrowUtc = startOfTodayUtc.AddDays(1);

            var todaysTotal = await _context.Transfers.Where(t =>
            t.CreatedAtUTC >= startOfTodayUtc &&
            t.CreatedAtUTC < startOfTommorrowUtc &&
            t.FromAccountId == fromAccountId)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            Transfer transfer = new Transfer
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = decimal.Round(amount, 2, MidpointRounding.ToZero),
                Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim(),
                CreatedAtUTC = DateTimeOffset.UtcNow
            };

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync(cancellationToken);

            return transfer;
        }

        Task<IReadOnlyList<Transfer>> ITransferService.GetByAccountAsync(int accountId, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
