using AnonymousData;
using Builder;
using FluentAssertions;
using MB.Common.Testing;
using MB.Data.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MB.Application.Api.E2ETests
{
    [TestClass]
    public class TransactionsControllerE2ETests : BaseE2ETest
    {
        private IEnumerable<Currency> Currencies;
        private Currency DefaultCurrency;
        private User User;
        private Account Account;

        [TestInitialize]
        public void TestInitialize()
        {
            Currencies = Seed<Currency>(c => c.ID, 3, (c, i) =>
            {
                switch (i)
                {
                    case 1:
                        c.Code = "GBP";
                        break;
                    case 2:
                        c.Code = "EUR";
                        break;
                    case 3:
                        c.Code = "USD";
                        break;
                }
            });

            DefaultCurrency = Currencies.First();
            User = SeedOne<User>(u => u.ID);
            Account = SeedOne<Account>(a => a.ID, (a, i) =>
            {
                a.UserId = User.ID;
                a.DefaultCurrencyId = DefaultCurrency.ID;
            });
        }

        [TestMethod]
        public async Task TransactionController_POST_should_create_transaction_and_return_its_location_and_content()
        {
            var expectedTransaction = Builder<Transaction>.New().Build(t =>
            {
                t.ID = default;
                t.DateTime = default;
                t.Amount = default;
                t.AccountId = Account.ID;
                t.CurrencyId = DefaultCurrency.ID;
            });

            var response = await _sutClient.PostAsync($"/api/transactions", expectedTransaction);
            
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var transaction = await response.Content.ReadAsAsync<Transaction>();
            transaction.Should().BeEquivalentTo(expectedTransaction, o => 
                o.Excluding(c => c.ID)
                .Excluding(c => c.Account)
                .Excluding(c => c.Currency)
                .Excluding(c => c.Amount)
                .Excluding(c => c.DateTime));

            transaction.DateTime.Should().NotBe(default);
            transaction.Amount.Should().Be(transaction.OriginalAmount);
            response.Headers.Location.ToString().Should().BeEquivalentTo($"/api/Transactions/{transaction.ID}");
        }

        [TestMethod]
        public async Task TransactionController_POST_should_not_allow_withdrawal_of_amounts_greater_than_balance()
        {
            var balance = Any.Decimal();
            var expectedTransactions = SeedOne<Transaction>(c => c.ID, (t, i) =>
            {
                t.AccountId = Account.ID;
                t.CurrencyId = DefaultCurrency.ID;
                t.DateTime = DateTime.UtcNow.AddMinutes(-1);
                t.OriginalAmount = balance;
                t.Amount = balance;
            });

            var expectedTransaction = Builder<Transaction>.New().Build(t =>
            {
                t.ID = default;
                t.DateTime = default;
                t.Amount = default;
                t.AccountId = Account.ID;
                t.CurrencyId = DefaultCurrency.ID;
                t.OriginalAmount = (balance + 0.01m) * -1m;
            });

            var response = await _sutClient.PostAsync($"/api/transactions", expectedTransaction);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should()
                .BeEquivalentTo("The Amount exceeds the current account balance");
        }
    }
}