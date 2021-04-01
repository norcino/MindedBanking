using Minded.CommandQuery.Query;
using Minded.Log;

namespace MB.Business.Account
{
    public class GetAccountBalanceByAccountIdQuery : IQuery<decimal>
    {
        public int Id { get; }

        public GetAccountBalanceByAccountIdQuery(int id)
        {
            Id = id;
        }

        public LogInfo ToLog()
        {
            return new LogInfo();
        }
    }
}
