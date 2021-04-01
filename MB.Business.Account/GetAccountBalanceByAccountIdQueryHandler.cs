using System.Linq;
using System.Threading.Tasks;
using MB.Data.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Account
{
    public class GetAccountBalanceByAccountIdQueryHandler : IQueryHandler<GetAccountBalanceByAccountIdQuery, decimal>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetAccountBalanceByAccountIdQuery, decimal>> _logger;

        public GetAccountBalanceByAccountIdQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetAccountBalanceByAccountIdQuery, decimal>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<decimal> HandleAsync(GetAccountBalanceByAccountIdQuery query)
        {
            return await _context.Transactions.Where(t => t.AccountId == query.Id).SumAsync(a => a.Amount);
        }
    } 
}
