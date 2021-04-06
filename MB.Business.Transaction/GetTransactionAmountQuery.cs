using Minded.CommandQuery.Query;
using Minded.Log;

namespace MB.Business.Transaction
{
    public class GetTransactionAmountQuery : IQuery<decimal>
    {
        public Data.Entities.Transaction Transaction { get; set; }

        public GetTransactionAmountQuery(Data.Entities.Transaction transaction)
        {
            Transaction = transaction;
        }

        public LogInfo ToLog()
        {
            return new LogInfo();
        }
    }
}
