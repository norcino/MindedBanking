using MB.Business.Currency;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class CurrencyController : BaseController
    {
        private readonly IMediator _mediator;

        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<Currency>> Get(ODataQueryOptions<Currency> queryOptions)
        {
            var query = ApplyODataQueryConditions<Currency, GetAllCurrenciesQuery>(queryOptions, new GetAllCurrenciesQuery());
            return await _mediator.ProcessQueryAsync(query);
        }
    }
}
