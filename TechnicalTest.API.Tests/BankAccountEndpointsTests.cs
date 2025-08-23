using Xunit;
using FluentAssertions;

using System.Net;
using System.Net.Http.Json;

using TechnicalTest.Data;
using TechnicalTest.API.Models.BankAccounts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace TechnicalTest.API.Tests
{
    public class BankAccountEndpointsTests : IClassFixture<CustomWebAppFactory>
    {
        private readonly CustomWebAppFactory _factory;

        public BankAccountEndpointsTests(CustomWebAppFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateBankAccount_Returns201_And_Payload()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Need a customer to attach to
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var cust = new Customer { Name = "Create Owner", DailyTransferLimit = 500m };
                db.Customers.Add(cust);
                await db.SaveChangesAsync();

                // Act
                var request = new CreateBankAccountRequest(cust.Id, "ACC-NEW-001");
                var response = await client.PostAsJsonAsync("/bankaccounts", request);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.Created);
                response.Headers.Location!.ToString().Should().MatchRegex(@"^/bankaccounts/\d+$");

                var body = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
                body.Should().NotBeNull();
                body!.CustomerId.Should().Be(cust.Id);
                body.AccountNumber.Should().Be("ACC-NEW-001");
                body.IsFrozen.Should().BeFalse();
                body.Balance.Should().Be(0m);
            }
        }

        [Fact]
        public async Task GetBankAccount_ById_Returns200()
        {
            // Arrange
            var client = _factory.CreateClient();

            int id;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var (_, a1, _) = await TestDataSeeder.SeedCustomerWithAccountsAsync(db);
                id = a1.Id;
            }

            // Act
            var response = await client.GetAsync($"/bankaccounts/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
            body!.Id.Should().Be(id);
        }

        [Fact]
        public async Task ListBankAccounts_Returns200_And_Items()
        {
            // Arrange
            var client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                await TestDataSeeder.SeedCustomerWithAccountsAsync(db);
            }

            // Act
            var response = await client.GetAsync("/bankaccounts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<BankAccountResponse>>();
            list.Should().NotBeNull();
            list!.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task UpdateBalance_Deposit_Returns200_And_UpdatesBalance()
        {
            // Arrange
            var client = _factory.CreateClient();
            int id;
            decimal initialBalance;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var (_, a1, _) = await TestDataSeeder.SeedCustomerWithAccountsAsync(db, a1Balance: 250m);
                id = a1.Id;
                initialBalance = a1.Balance;
            }

            // Act
            var payload = new UpdateBalanceRequest(100m); // deposit +100
            var response = await client.PostAsJsonAsync($"/bankaccounts/{id}/balance", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<BankAccountResponse>();
            body!.Balance.Should().Be(initialBalance + 100m);
        }

        [Fact]
        public async Task UpdateBalance_Withdraw_InsufficientFunds_Returns400_Problem()
        {
            // Arrange
            var client = _factory.CreateClient();
            int id;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var (_, a1, _) = await TestDataSeeder.SeedCustomerWithAccountsAsync(db, a1Balance: 50m);
                id = a1.Id;
            }

            // Act
            var payload = new UpdateBalanceRequest(-200m); // withdraw more than balance
            var response = await client.PostAsJsonAsync($"/bankaccounts/{id}/balance", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
            problem!.Title.Should().Be("Validation failed"); // matches Results.Problem(...) in your endpoint
            problem.Detail.Should().Contain("Insufficient funds");
        }
    }
}
