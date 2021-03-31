using Minded.CommandQuery.Query;
using Minded.CommandQuery.Query.Trait;
using Minded.Common.Query.Trait;
using Minded.Log;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MB.Business.Currency
{
    public class GetAllCurrenciesQuery : IQuery<List<Data.Entities.Currency>>, ICanCount, ICanTop, ICanSkip, ICanExpand, ICanOrderBy, ICanFilter<Data.Entities.Currency>
    {
        public LogInfo ToLog()
        {
            return new LogInfo();
        }

        public int? Top { get; set; }
        public int Skip { get; set; }
        public string[] Expand { get; set; }
        public bool Count { get; set; }
        public IList<OrderDescriptor> OrderBy { get; set; }
        public Expression<Func<Data.Entities.Currency, bool>> Filter { get; set; }
    }
}
