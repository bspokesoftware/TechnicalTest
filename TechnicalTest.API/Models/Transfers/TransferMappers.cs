using TechnicalTest.Data;

namespace TechnicalTest.API.Models.Transfers
{
    public static class TransferMappers
    {
        public static TransferResponse ToResponse(this Transfer t) =>
            new(
                t.Id,
                t.FromAccountId,
                t.ToAccountId,
                t.Amount,
                t.Reference,
                t.CreatedAtUTC
            );
    }
}
