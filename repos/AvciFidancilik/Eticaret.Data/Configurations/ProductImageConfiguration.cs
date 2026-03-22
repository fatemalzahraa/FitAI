using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            // Tablo adı
            builder.ToTable("ProductImages");

            // Primary Key
            builder.HasKey(pi => pi.Id);

            // Alan özellikleri
            builder.Property(pi => pi.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(pi => pi.IsMain)
                   .IsRequired();

            builder.Property(pi => pi.ProductId)
                   .IsRequired();

            // ProductImage – Product (N - 1)
            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.ProductImages) // Product sınıfında ICollection<ProductImage> ProductImages olmalı
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
