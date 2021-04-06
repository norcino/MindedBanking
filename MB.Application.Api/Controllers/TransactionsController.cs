using MB.Business.Transaction;
using MB.Business.User;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Transaction transaction)
        {
            var command = new CreateTransactionCommand(transaction);
            var response = await _mediator.ProcessCommandAsync<int>(command);

            if(!response.Successful)
            {
                return BadRequest(response.ValidationEntries.First()?.ToString() ?? "Operation not allowed");
            }

            return Created($"/api/Transactions/{transaction.ID}", transaction);
        }
    }
}
