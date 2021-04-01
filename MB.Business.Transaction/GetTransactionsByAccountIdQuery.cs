using Minded.CommandQuery.Query;
using Minded.CommandQuery.Query.Trait;
using Minded.Common.Query.Trait;
using Minded.Log;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MB.Business.Transaction
{
    public class GetTransactionsByAccountIdQuery : IQuery<List<Data.Entities.Transaction>>, ICanCount, ICanTop, ICanSkip, ICanExpand, ICanOrderBy, ICanFilter<Data.Entities.Transaction>
    {
        public int AccountId;

        public GetTransactionsByAccountIdQuery(int accountId)
        {
            AccountId = accountId;
        }

        public LogInfo ToLog()
        {
            return new LogInfo();
        }

        public int? Top { get; set; }
        public int Skip { get; set; }
        public string[] Expand { get; set; }
        public bool Count { get; set; }
        public IList<OrderDescriptor> OrderBy { get; set; }
        public Expression<Func<Data.Entities.Transaction, bool>> Filter { get; set; }
    }
}
