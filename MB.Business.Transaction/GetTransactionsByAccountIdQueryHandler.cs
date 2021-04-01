using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Transaction
{
    public class GetTransactionsByAccountIdQueryHandler : IQueryHandler<GetTransactionsByAccountIdQuery, List<Data.Entities.Transaction>>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetTransactionsByAccountIdQuery, List<Data.Entities.Transaction>>> _logger;

        public GetTransactionsByAccountIdQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetTransactionsByAccountIdQuery, List<Data.Entities.Transaction>>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<List<Data.Entities.Transaction>> HandleAsync(GetTransactionsByAccountIdQuery query)
        {
            return await query.ApplyTo(_context.Transactions.Where(t => t.AccountId == query.AccountId).AsQueryable()).ToListAsync();
        }
    } 
}
