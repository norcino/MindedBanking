using Minded.CommandQuery.Query;
using Minded.Log;

namespace MB.Business.Exchange
{
    public class GetExchangeRateQuery : IQuery<decimal>
    {
        public string BaseCurrencyCode { get; }
        public string TargetCurrencyCode { get; }
        public decimal Amount { get; }

        public GetExchangeRateQuery(string baseCurrencyCode, string targetCurrencyCode, decimal amount)
        {
            BaseCurrencyCode = baseCurrencyCode;
            TargetCurrencyCode = targetCurrencyCode;
            Amount = amount;
        }

        public LogInfo ToLog()
        {
            return new LogInfo();
        }
    }
}
