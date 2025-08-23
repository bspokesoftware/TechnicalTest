using Microsoft.AspNetCore.Mvc;

using TechnicalTest.Data;
using TechnicalTest.Data.Services.Transfers;

using TechnicalTest.API.Models.Transfers;

namespace TechnicalTest.API.Endpoints;

public static class TransferEndpoints
{
    public static IEndpointRouteBuilder MapTransferEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/transfers").WithTags("Transfers");

        group.MapPost("/", CreateTransfer)
             .WithName("CreateTransfer")
             .Produces<TransferResponse>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", GetTransfers)
             .WithName("GetTransfers")
             .Produces<List<TransferResponse>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateTransfer(
        [FromBody] CreateTransferRequest request,
        [FromServices] ITransferService service,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        try
        {
            Transfer created = await service.CreateAsync(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount,
                request.Reference,
                cancellationToken);

            TransferResponse dto = created.ToResponse();
            return Results.Created($"/transfers/{dto.Id}", dto);
        }
        catch (TransferException ex) when (ex.ErrorCode == TransferErrorCode.ValidationFailed)
        {
            return Results.Problem(
                title: "Validation failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (TransferException ex) when (ex.ErrorCode == TransferErrorCode.NotFound)
        {
            return Results.Problem(
                title: "Resource not found",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (TransferException ex) when (ex.ErrorCode == TransferErrorCode.BusinessRuleViolation)
        {
            return Results.Problem(
                title: "Business rule violation",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict);
        }
    }

    private static async Task<IResult> GetTransfers(
        [FromQuery] int accountId,
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        [FromServices] ITransferService service,
        CancellationToken ct)
    {
        try
        {
            var list = await service.GetByAccountAsync(accountId, fromUtc, toUtc, ct);
            var dto = list.Select(t => t.ToResponse()).ToList();
            return Results.Ok(dto);
        }
        catch (TransferException ex) when (ex.ErrorCode == TransferErrorCode.ValidationFailed)
        {
            return Results.Problem(
                title: "Validation failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (TransferException ex) when (ex.ErrorCode == TransferErrorCode.NotFound)
        {
            return Results.Problem(
                title: "Resource not found",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound);
        }
    }
}
