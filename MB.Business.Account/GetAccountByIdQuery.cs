using Minded.CommandQuery.Query;
using Minded.Common.Query.Trait;
using Minded.Log;

namespace MB.Business.Account
{
    public class GetAccountByIdQuery : IQuery<Data.Entities.Account>, ICanExpand
    {
        public int Id { get; }

        public GetAccountByIdQuery(int id)
        {
            Id = id;
        }

        public LogInfo ToLog()
        {
            return new LogInfo();
        }

        public string[] Expand { get; set; }
    }
}
