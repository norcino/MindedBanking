using MB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MB.Data.Access.Mapping
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.ID);
            builder.Property(u => u.Name).HasColumnType("nvarchar(150)");
            builder.Property(u => u.Surname).HasColumnType("nvarchar(150)");
            //builder.HasOne(u => u.Account)
            //    .WithOne(a => a.User)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .HasConstraintName("FK_Account_User");
        }
    }
}
