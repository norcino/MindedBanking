using MB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MB.Data.Access
{
    public interface IMindedBankingContext : IDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<Transaction> Currencies { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<T> Set<T>() where T : class, new();
    }
}
