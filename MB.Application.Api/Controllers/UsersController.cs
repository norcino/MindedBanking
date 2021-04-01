using MB.Business.User;
using MB.Data.Entities;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Minded.Mediator;
using System.Threading.Tasks;

namespace MB.Application.Api
{
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetByUserId")]
        public async Task<IActionResult> Get(int id, ODataQueryOptions<User> queryOptions)
        {
            var query = ApplyODataQueryConditions<User, GetUserByIdQuery>(queryOptions, new GetUserByIdQuery(id));
            var result = await _mediator.ProcessQueryAsync(query);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}
