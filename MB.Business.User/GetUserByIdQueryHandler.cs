using System.Linq;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.User
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Data.Entities.User>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetUserByIdQuery, Data.Entities.User>> _logger;

        public GetUserByIdQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetUserByIdQuery, Data.Entities.User>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<Data.Entities.User> HandleAsync(GetUserByIdQuery query)
        {
            return await query.ApplyTo(_context.Users.Where(u => u.ID == query.Id).AsQueryable()).FirstOrDefaultAsync();
        }
    } 
}
