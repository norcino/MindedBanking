using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MB.Data.Access
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync();

        IModel Model { get; }
    }
}
