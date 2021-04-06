using System;
using System.Threading.Tasks;
using MB.Business.Account;
using MB.Business.Currency;
using MB.Business.Exchange;
using MB.Data.Access;
using Minded.CommandQuery.Query;
using Minded.Mediator;

namespace MB.Business.Transaction
{
    public class GetTransactionAmountQueryHandler : IQueryHandler<GetTransactionAmountQuery, decimal>
    {
        private readonly IMediator _mediator;

        public GetTransactionAmountQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<decimal> HandleAsync(GetTransactionAmountQuery query)
        {
            var getAccountQuery = new GetAccountByIdQuery(query.Transaction.AccountId);
            var account = await _mediator.ProcessQueryAsync(getAccountQuery);

            var getCurrencyByIdQuery = new GetCurrencyByIdQuery(account.DefaultCurrencyId);
            var defaultCurrency = await _mediator.ProcessQueryAsync(getCurrencyByIdQuery);

            query.Transaction.DateTime = DateTime.UtcNow;

            if (query.Transaction.CurrencyId != defaultCurrency.ID)
            {
                getCurrencyByIdQuery = new GetCurrencyByIdQuery(query.Transaction.CurrencyId);
                var transactionCurrency = await _mediator.ProcessQueryAsync(getCurrencyByIdQuery);

                var getExchangeRateQuery = new GetExchangeRateQuery(transactionCurrency.Code, defaultCurrency.Code, query.Transaction.Amount);
                var exchangeRate = await _mediator.ProcessQueryAsync(getExchangeRateQuery);

                return Math.Round(exchangeRate * query.Transaction.OriginalAmount, 2);
            }

            return Math.Round(query.Transaction.OriginalAmount, 2);
        }
    } 
}
