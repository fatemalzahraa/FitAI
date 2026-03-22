using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class AddressConfigurations : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // Tablo adı
            builder.ToTable("Addresses");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Zorunlu ve opsiyonel alan ayarları
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Line)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PostalCode)
                .HasMaxLength(20);

            builder.Property(x => x.IdentityNumber)
                .HasMaxLength(11);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            // User – Address (1 - N)
            builder.HasOne(x => x.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
