using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Tablo adı
            builder.ToTable("Roles");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Alan özellikleri
            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Role – UserRole (1 - N)
            builder.HasMany(r => r.UserRoles)        // Rolün birden fazla UserRole kaydı olur
                   .WithOne(ur => ur.Role)          // UserRole içinde Role navigasyonu olmalı
                   .HasForeignKey(ur => ur.RoleId)  // UserRole tablosunda RoleId olacak
                   .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}
