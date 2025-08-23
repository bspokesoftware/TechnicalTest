namespace TechnicalTest.API.Models.Transfers
{
    public sealed record TransferResponse(int Id, int FromAccountId, int ToAccountId, decimal Amount, string? Reference, DateTimeOffset CreatedAtUtc);
}
