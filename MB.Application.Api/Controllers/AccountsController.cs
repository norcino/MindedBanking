using MB.Business.Account;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var query = new GetAccountBalanceByAccountIdQuery(id);
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
