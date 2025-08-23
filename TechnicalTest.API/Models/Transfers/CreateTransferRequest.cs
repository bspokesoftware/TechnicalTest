namespace TechnicalTest.API.Models.Transfers
{
    public sealed record CreateTransferRequest(int FromAccountId, int ToAccountId, decimal Amount, string? Reference);
}
