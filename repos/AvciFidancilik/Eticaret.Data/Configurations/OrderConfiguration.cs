using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Tablo adı
            builder.ToTable("Orders");

            // Primary Key
            builder.HasKey(o => o.Id);

            // Alan özellikleri
            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.OrderDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Order – User (N - 1)
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders) // AppUser sınıfında public ICollection<Order> Orders olmalı
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Order – OrderItems (1 - N)
            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order) // OrderItem sınıfında public Order Order { get; set; } olmalı
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
