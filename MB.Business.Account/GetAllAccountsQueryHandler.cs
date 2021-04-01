using System.Collections.Generic;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Account
{
    public class GetAllAccountsQueryHandler : IQueryHandler<GetAllAccountsQuery, List<Data.Entities.Account>>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetAllAccountsQuery, List<Data.Entities.Account>>> _logger;

        public GetAllAccountsQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetAllAccountsQuery, List<Data.Entities.Account>>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<List<Data.Entities.Account>> HandleAsync(GetAllAccountsQuery query)
        {
            return await query.ApplyTo(_context.Accounts.AsQueryable()).ToListAsync();
        }
    } 
}
