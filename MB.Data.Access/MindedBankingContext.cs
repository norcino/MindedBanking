using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MB.Data.Access
{
    public class MindedBankingContext : BaseContext<MindedBankingContext>, IMindedBankingContext
    {
        public MindedBankingContext(DbContextOptions<MindedBankingContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Account> Accounts { get; set; }

        //public new DbSet<T> Set<T>() where T : class, new()
        //{
        //    return base.Set<T>();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            // Get all mappings from the current assembly
            var mappingTypes = Assembly.GetAssembly(GetType())
                .GetTypes()
                .Where(t => t.GetInterfaces()
                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods().Single(x =>
                x.Name == "ApplyConfiguration" &&
                x.IsGenericMethod &&
                x.GetParameters().FirstOrDefault()?.ParameterType.Name == "IEntityTypeConfiguration`1"
            );

            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                // Create the method using the generic type
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

                // Invoke the mapping method
                genericEntityMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(mappingType) });
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}