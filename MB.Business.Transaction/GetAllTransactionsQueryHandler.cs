using System.Collections.Generic;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Transaction
{
    public class GetAllTransactionsQueryHandler : IQueryHandler<GetAllTransactionsQuery, List<Data.Entities.Transaction>>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetAllTransactionsQuery, List<Data.Entities.Transaction>>> _logger;

        public GetAllTransactionsQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetAllTransactionsQuery, List<Data.Entities.Transaction>>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<List<Data.Entities.Transaction>> HandleAsync(GetAllTransactionsQuery query)
        {
            return await query.ApplyTo(_context.Transactions.AsQueryable()).ToListAsync();
        }
    } 
}
