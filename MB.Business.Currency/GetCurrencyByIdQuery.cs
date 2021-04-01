using Minded.CommandQuery.Query;
using Minded.Common.Query.Trait;
using Minded.Log;

namespace MB.Business.Currency
{
    public class GetCurrencyByIdQuery : IQuery<Data.Entities.Currency>, ICanExpand
    {
        public int Id { get; }

        public GetCurrencyByIdQuery(int id)
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
