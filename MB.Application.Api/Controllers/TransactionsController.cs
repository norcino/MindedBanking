using MB.Business.Transaction;
using MB.Business.User;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Common;
using Minded.Mediator;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class TransactionsController : BaseController
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetByAccountId")]
        public async Task<IActionResult> GetByAccountId(int id, ODataQueryOptions<Transaction> queryOptions)
        {
            var query = ApplyODataQueryConditions<User, GetTransactionsByAccountIdQuery>(queryOptions, new GetTransactionsByAccountIdQuery(id));
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Transaction transaction)
        {
            var command = new CreateTransactionCommand(transaction);
            var response = await _mediator.ProcessCommandAsync<CommandResponse<int>>(command);

            if(!response.Successful)
            {
                return BadRequest(response.ValidationEntries);
            }

            return Created($"/api/Tranzactions/{transaction.ID}", transaction);
        }
    }
}
