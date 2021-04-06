using Minded.CommandQuery.Query;
using Minded.Common.Query.Trait;
using Minded.Log;

namespace MB.Business.User
{
    public class GetUserByIdQuery : IQuery<Data.Entities.User>, ICanExpand
    {
        public int Id { get; }

        public GetUserByIdQuery(int id)
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
