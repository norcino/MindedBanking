using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MB.Data.Access
{
    public class BaseContext<TContext> : DbContext where TContext : DbContext
    {
        protected BaseContext()
        { }

        protected BaseContext(DbContextOptions options) : base(options)
        {
        }

        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

        public new IModel Model => base.Model;

        public override void Dispose()
        {
            Database?.CloseConnection();
        }
    }
}