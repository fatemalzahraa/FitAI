using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Tablo adı
            builder.ToTable("OrderItems");

            // Primary Key
            builder.HasKey(oi => oi.Id);

            // Alan özellikleri
            builder.Property(oi => oi.OrderId)
                   .IsRequired();

            builder.Property(oi => oi.ProductId)
                   .IsRequired();

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // OrderItem – Order (N - 1)
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems) // Order sınıfında ICollection<OrderItem> OrderItems olmalı
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // OrderItem – Product (N - 1)
            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems) // Product sınıfında ICollection<OrderItem> OrderItems olmalı
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Seed Data (opsiyonel, örnek)
           
        }
    }
}
