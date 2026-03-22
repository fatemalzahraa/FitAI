using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Tablo adı
            builder.ToTable("Categories");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Alan özellikleri
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Category – Product (1 - N)
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category) // Product sınıfında public Category Category property olmalı
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

          
        }
    }
}
