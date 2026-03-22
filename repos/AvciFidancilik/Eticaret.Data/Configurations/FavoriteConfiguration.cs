using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class FavoriteConfigurations : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            // Tablo adı
            builder.ToTable("Favorites");

            // Primary Key
            builder.HasKey(f => f.Id);

            // Alan özellikleri
            builder.Property(f => f.UserId)
                   .IsRequired();

            builder.Property(f => f.ProductId)
                   .IsRequired();

            // Favorite – User (N - 1)
            builder.HasOne(f => f.User)
                   .WithMany(u => u.Favorites) // AppUser sınıfında public ICollection<Favorite> Favorites olmalı
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Favorite – Product (N - 1)
            builder.HasOne(f => f.Product)
                   .WithMany(p => p.Favorites) // Product sınıfında public ICollection<Favorite> Favorites olmalı
                   .HasForeignKey(f => f.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            
           
        }
    }
}
