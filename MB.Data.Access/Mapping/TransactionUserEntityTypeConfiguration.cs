using MB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MB.Data.Access.Mapping
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(u => u.ID);
            builder.Property(u => u.DateTime).HasColumnType("datetime");
            builder.Property(u => u.Amount).HasColumnType("money");
            builder.Property(u => u.OriginalAmount).HasColumnType("money");
            builder.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.CurrencyId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
