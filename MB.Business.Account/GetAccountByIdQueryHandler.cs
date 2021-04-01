using System.Linq;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Account
{
    public class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, Data.Entities.Account>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetAccountByIdQuery, Data.Entities.Account>> _logger;

        public GetAccountByIdQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetAccountByIdQuery, Data.Entities.Account>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<Data.Entities.Account> HandleAsync(GetAccountByIdQuery query)
        {
            return await query.ApplyTo(_context.Accounts.Where(u => u.ID == query.Id).AsQueryable()).FirstOrDefaultAsync();
        }
    } 
}
