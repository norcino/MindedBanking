using MB.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MB.Data.Access
{
    // TODO: Remove only for demostration purpose
    public static class SampleDataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = (IMindedBankingContext)serviceProvider.GetService(typeof(IMindedBankingContext));

            if (context.Currencies.Any()) return;

            var user = new User { Name = "Joe", Surname = "Wolf" };
            var gbpCurrency = new Currency { Code = "GBP", Name = "Pound Sterling" };
            var usdCurrency  = new Currency { Code = "USD", Name = "US Dollar" };
            var currencies = new List<Currency>
            {
                new Currency { Code = "EUR", Name = "Euro" },
                new Currency { Code = "CHF", Name = "Swiss Franc" }
            };

            var account = new Account { Number = 1, User = user, DefaultCurrency = gbpCurrency };

            var transactions = new List<Transaction> {
                new Transaction { Account = account, Currency = gbpCurrency, Amount = 500m, OriginalAmount = 500m, Description = "Bank transfer 123456", DateTime = DateTime.UtcNow.AddMinutes(-5) },
                new Transaction { Account = account, Currency = gbpCurrency, Amount = -100.10m, OriginalAmount = -100.10m, Description = "Visa transaction 1234", DateTime = DateTime.UtcNow.AddMinutes(-4) },
                new Transaction { Account = account, Currency = usdCurrency, Amount = -50m, OriginalAmount = -68.93m, Description = "International ATM withdrawal", DateTime = DateTime.UtcNow.AddMinutes(-3) },
                new Transaction { Account = account, Currency = gbpCurrency, Amount = -15m, OriginalAmount = -15m, Description = "Direct debit Spotify", DateTime = DateTime.UtcNow.AddMinutes(-2) },
                new Transaction { Account = account, Currency = gbpCurrency, Amount = -3m, OriginalAmount = -3m, Description = "Yearly account fees", DateTime = DateTime.UtcNow.AddMinutes(-1) },
                new Transaction { Account = account, Currency = gbpCurrency, Amount = -40m, OriginalAmount = -40m, Description = "Credit card renewal", DateTime = DateTime.UtcNow }
            };

            context.Currencies.AddRange(currencies);
            context.Transactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
