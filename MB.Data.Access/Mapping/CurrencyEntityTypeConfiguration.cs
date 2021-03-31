using MB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MB.Data.Access.Mapping
{
    public class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(u => u.ID);
            builder.Property(u => u.Name).HasColumnType("nvarchar(35)");
            builder.Property(u => u.Code).HasColumnType("nvarchar(5)");
        }
    }
}
