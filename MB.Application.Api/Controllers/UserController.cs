using MB.Business.User;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> Get(int id, ODataQueryOptions<User> queryOptions)
        {
            var query = ApplyODataQueryConditions<User, GetUserByIdQuery>(queryOptions, new GetUserByIdQuery(id));
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
