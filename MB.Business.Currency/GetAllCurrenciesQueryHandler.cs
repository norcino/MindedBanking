using System.Collections.Generic;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Currency
{
    public class GetAllCurrenciesQueryHandler : IQueryHandler<GetAllCurrenciesQuery, List<Data.Entities.Currency>>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetAllCurrenciesQuery, List<Data.Entities.Currency>>> _logger;

        public GetAllCurrenciesQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetAllCurrenciesQuery, List<Data.Entities.Currency>>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<List<Data.Entities.Currency>> HandleAsync(GetAllCurrenciesQuery query)
        {
            return await query.ApplyTo(_context.Currencies.AsQueryable()).ToListAsync();
        }
    } 
}
