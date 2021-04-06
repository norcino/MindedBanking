using MB.Business.Account;
using MB.Business.Transaction;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;            
        }

        [HttpGet()]
        public async Task<IActionResult> Get(ODataQueryOptions<Account> queryOptions)
        {
            var query = ApplyODataQueryConditions<ICollection<Account>, GetAllAccountsQuery>(queryOptions, new GetAllAccountsQuery());
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, ODataQueryOptions<Account> queryOptions)
        {
            var query = ApplyODataQueryConditions<Account, GetAccountByIdQuery>(queryOptions, new GetAccountByIdQuery(id));
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}/Balance", Name = "GetBalanceByAccountId")]
        public async Task<IActionResult> GetBalanceByAccountId(int id)
        {
            var getAccountQuery = new GetAccountByIdQuery(id);
            var account = await _mediator.ProcessQueryAsync(getAccountQuery);
            if (account == null) return NotFound();

            var query = new GetAccountBalanceByAccountIdQuery(id);
            var result = await _mediator.ProcessQueryAsync(query);

            return Ok(result);
        }

        [HttpGet("{id}/Transactions", Name = "GetTransactionsByAccountId")]
        public async Task<IActionResult> GetTransactionsByAccountId(int id, ODataQueryOptions<Transaction> queryOptions)
        {
            var getAccountQuery = new GetAccountByIdQuery(id);
            var account = await _mediator.ProcessQueryAsync(getAccountQuery);
            if (account == null) return NotFound();

            var query = ApplyODataQueryConditions<List<Transaction>, GetTransactionsByAccountIdQuery>(queryOptions, new GetTransactionsByAccountIdQuery(id));
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
