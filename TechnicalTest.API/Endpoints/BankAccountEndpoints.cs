using Microsoft.AspNetCore.Mvc;

using TechnicalTest.API.Models.BankAccounts;

using TechnicalTest.Data;
using TechnicalTest.Data.Services.BankAccounts;

namespace TechnicalTest.API.Endpoints
{
    public static class BankAccountEndpoints
    {
        public static IEndpointRouteBuilder MapBankAccountEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/bankaccounts").WithTags("Bank Accounts");

            group.MapPost("/", Create)
                 .WithName("CreateBankAccount")
                 .Produces<BankAccountResponse>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status409Conflict);

            group.MapPost("/{id:int}/balance", UpdateBalance)
                 .WithName("UpdateBankAccountBalance")
                 .Produces<BankAccountResponse>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapGet("/", List)
                 .WithName("ListBankAccounts")
                 .Produces<List<BankAccountResponse>>(StatusCodes.Status200OK);

            group.MapGet("/{id:int}", GetById)
                 .WithName("GetBankAccount")
                 .Produces<BankAccountResponse>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPatch("/{id:int}", UpdateFrozen)
                 .WithName("UpdateBankAccount")
                 .Produces<BankAccountResponse>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapDelete("/{id:int}", Delete)
                 .WithName("DeleteBankAccount")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        private static async Task<IResult> Create(
            [FromBody] CreateBankAccountRequest request,
            [FromServices] IBankAccountService service,
            CancellationToken cancellationToken)
        {
            try
            {
                BankAccount created = await service.CreateAsync(request.CustomerId, request.AccountNumber, cancellationToken);
                BankAccountResponse dto = created.ToResponse();
                return Results.Created($"/bankaccounts/{dto.Id}", dto);
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.ValidationFailed)
            {
                return Results.Problem(
                    title: "Validation failed", 
                    detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.NotFound)
            {
                return Results.Problem(
                    title: "Resource not found",
                    detail: ex.Message, 
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.Conflict)
            {
                return Results.Problem(
                    title: "Conflict", 
                    detail: ex.Message, 
                    statusCode: StatusCodes.Status409Conflict);
            }
        }

        private static async Task<IResult> List(
            [FromServices] IBankAccountService service,
            CancellationToken ct)
        {
            var accounts = await service.GetAllAsync(ct);
            var dto = accounts.Select(a => a.ToResponse()).ToList();
            return Results.Ok(dto);
        }

        private static async Task<IResult> GetById(
            [FromRoute] int id,
            [FromServices] IBankAccountService service,
            CancellationToken ct)
        {
            try
            {
                BankAccount account = await service.GetByIdAsync(id, ct);
                return Results.Ok(account.ToResponse());
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.NotFound)
            {
                return Results.Problem(
                    title: "Resource not found", 
                    detail: ex.Message, 
                    statusCode: StatusCodes.Status404NotFound);
            }
        }

        private static async Task<IResult> UpdateFrozen(
            [FromRoute] int id,
            [FromBody] UpdateBankAccountRequest request,
            [FromServices] IBankAccountService service,
            CancellationToken cancellationToken)
        {
            try
            {
                BankAccount updated = await service.UpdateFrozenAsync(id, request.IsFrozen, cancellationToken);
                return Results.Ok(updated.ToResponse());
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.ValidationFailed)
            {
                return Results.Problem(
                    title: "Validation failed", 
                    detail: ex.Message, 
                    statusCode: StatusCodes.Status400BadRequest);
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.NotFound)
            {
                return Results.Problem(
                    title: "Resource not found", 
                    detail: ex.Message, statusCode: 
                    StatusCodes.Status404NotFound);
            }
        }

        private static async Task<IResult> UpdateBalance(
            [FromRoute] int id,
            [FromBody] UpdateBalanceRequest request,
            [FromServices] IBankAccountService service,
            CancellationToken cancellationToken)
        {
            try
            {
                BankAccount updated = await service.UpdateBalanceAsync(id, request.Amount, cancellationToken);
                return Results.Ok(updated.ToResponse());
            }
            catch(BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.ValidationFailed)
            {
                return Results.Problem(
                    title: "Validation failed", 
                    detail: ex.Message, 
                    statusCode: StatusCodes.Status400BadRequest);
            }
            catch(BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.NotFound)
            {
                return Results.Problem(
                    title: "Resource not found", 
                    detail: ex.Message, statusCode: 
                    StatusCodes.Status404NotFound);
            }
        }

        private static async Task<IResult> Delete(
            [FromRoute] int id,
            [FromServices] IBankAccountService service,
            CancellationToken ct)
        {
            try
            {
                await service.DeleteAsync(id, ct);
                return Results.NoContent();
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.NotFound)
            {
                return Results.Problem(title: "Resource not found", detail: ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (BankAccountException ex) when (ex.ErrorCode == BankAccountErrorCode.Conflict)
            {
                return Results.Problem(title: "Conflict", detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}