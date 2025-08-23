using TechnicalTest.Data;

namespace TechnicalTest.API.Tests
{
    public static class TestDataSeeder
    {
        public static async Task<(Customer customer, BankAccount a1, BankAccount a2)> SeedCustomerWithAccountsAsync(
            ApplicationContext db,
            decimal a1Balance = 500m,
            decimal a2Balance = 100m,
            decimal dailyLimit = 1_000m)
        {
            var customer = new Customer
            {
                Name = "Test User",
                DailyTransferLimit = dailyLimit
            };

            var a1 = new BankAccount
            {
                Customer = customer,
                AccountNumber = "ACC-001",
                IsFrozen = false,
                Balance = a1Balance
            };

            var a2 = new BankAccount
            {
                Customer = customer,
                AccountNumber = "ACC-002",
                IsFrozen = false,
                Balance = a2Balance
            };

            db.Customers.Add(customer);
            db.BankAccounts.AddRange(a1, a2);
            await db.SaveChangesAsync();

            return (customer, a1, a2);
        }
    }
}