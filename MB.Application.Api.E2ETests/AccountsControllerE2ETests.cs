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
    public class AccountsControllerE2ETests : BaseE2ETest
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
        public async Task AccountController_should_return_200OK_with_the_list_of_transactions_when_navigationg_from_account_to_Transactions()
        {
            var dateTime = DateTime.UtcNow.AddDays(-5);
            dateTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            var expectedTransactions = Seed<Transaction>(c => c.ID, 50, (t, i) =>
            {
                t.AccountId = Account.ID;
                t.CurrencyId = DefaultCurrency.ID;
                t.DateTime = dateTime.AddHours(i);
            });

            var response = await _sutClient.GetAsync($"/api/accounts/{Account.ID}/transactions");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var transactions = await response.Content.ReadAsAsync<List<Transaction>>();
            Assert.AreEqual(expectedTransactions.Count(), transactions.Count);

            transactions.Should().BeEquivalentTo(expectedTransactions, o => o.Excluding(c => c.ID).Excluding(c => c.Account).Excluding(c => c.Currency));
        }

        [TestMethod]
        public async Task AccountController_should_return_200OK_with_the_empty_list_of_transactions_if_none_available_for_the_account_when_navigationg_from_account_to_Transactions()
        {
            var response = await _sutClient.GetAsync($"/api/accounts/{Account.ID}/transactions");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var transactions = await response.Content.ReadAsAsync<List<Transaction>>();
            transactions.Should().BeEmpty();
        }
    }
}