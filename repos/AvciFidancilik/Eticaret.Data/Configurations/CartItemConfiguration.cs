using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class CartItemConfigurations : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Tablo adı
            builder.ToTable("CartItems");

            // Primary Key
            builder.HasKey(ci => ci.Id);

            // Alan özellikleri
            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            // CartItem – Cart (N - 1)
            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems) // Cart sınıfında ICollection<CartItem> CartItems olmalı
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // CartItem – Product (N - 1)
            builder.HasOne(ci => ci.Product)
                   .WithMany(p => p.CartItems) // Product sınıfında ICollection<CartItem> CartItems olmalı
                   .HasForeignKey(ci => ci.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}
