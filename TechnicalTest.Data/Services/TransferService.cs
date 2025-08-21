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
            int formAccountId,
            int toAccountId,
            decimal amount,
            string? reference,
            CancellationToken cancellationToken = default
            )
        {

        }
    }
}
