using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;
using Newtonsoft.Json;

namespace MB.Business.Exchange
{
    // TODO pass as configuration parameter the url of the exchange service
    public class GetExchangeRateQueryHandler : IQueryHandler<GetExchangeRateQuery, decimal>
    {
        private readonly ILogger<IQueryHandler<GetExchangeRateQuery, decimal>> _logger;
        private readonly HttpMessageHandler _httpHandler;

        public GetExchangeRateQueryHandler(ILogger<IQueryHandler<GetExchangeRateQuery, decimal>> logger)
        {
            _logger = logger;
        }

        public GetExchangeRateQueryHandler(HttpMessageHandler httpHandler, ILogger<IQueryHandler<GetExchangeRateQuery, decimal>> logger)
        {
            _logger = logger;
            _httpHandler = httpHandler;
        }

        // HttpClient here could be reused for optimisation
        public async Task<decimal> HandleAsync(GetExchangeRateQuery query)
        {
            var urlTemplate = $"https://api.ratesapi.io/api/latest?base={query.BaseCurrencyCode}&symbols={query.TargetCurrencyCode}";

            using (var httpClient = _httpHandler == null ? new HttpClient() : new HttpClient(_httpHandler))
            {
                var response = await httpClient.GetAsync(urlTemplate);
                var content = await response.Content.ReadAsStringAsync();

                var exchangeRateServiceResponse = JsonConvert.DeserializeObject<ExchangeRateServiceResponse>(content);

                var value = exchangeRateServiceResponse.rates[query.TargetCurrencyCode];
                return value;
            }
        }
    }

    internal class ExchangeRateServiceResponse
    {
        public string date;
        public string @base;
        public Dictionary<string,decimal> rates;
    }
}
