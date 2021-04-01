using System.Linq;
using System.Threading.Tasks;
using MB.Data.Access;
using MB.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minded.CommandQuery.Query;

namespace MB.Business.Currency
{
    public class GetCurrencyByIdQueryHandler : IQueryHandler<GetCurrencyByIdQuery, Data.Entities.Currency>
    {
        private readonly IMindedBankingContext _context;
        private readonly ILogger<IQueryHandler<GetCurrencyByIdQuery, Data.Entities.Currency>> _logger;

        public GetCurrencyByIdQueryHandler(IMindedBankingContext context, ILogger<IQueryHandler<GetCurrencyByIdQuery, Data.Entities.Currency>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public async Task<Data.Entities.Currency> HandleAsync(GetCurrencyByIdQuery query)
        {
            return await query.ApplyTo(_context.Currencies.Where(u => u.ID == query.Id).AsQueryable()).FirstOrDefaultAsync();
        }
    } 
}
