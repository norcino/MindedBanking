using MB.Business.Currency;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class CurrenciesController : BaseController
    {
        private readonly IMediator _mediator;

        public CurrenciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(ODataQueryOptions<Currency> queryOptions)
        {
            var query = ApplyODataQueryConditions<Currency, GetAllCurrenciesQuery>(queryOptions, new GetAllCurrenciesQuery());
            return Ok(await _mediator.ProcessQueryAsync(query));
        }
    }
}
