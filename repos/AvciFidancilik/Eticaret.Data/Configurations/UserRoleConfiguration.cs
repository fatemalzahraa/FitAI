using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Tablo adı
            builder.ToTable("UserRoles");

            // Primary Key
            builder.HasKey(ur => ur.Id);

            // UserId zorunlu
            builder.Property(ur => ur.UserId)
                   .IsRequired();

            // RoleId zorunlu
            builder.Property(ur => ur.RoleId)
                   .IsRequired();

            // UserRole → AppUser (N - 1)
            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles) // AppUser’da ICollection<UserRole> UserRoles olmalı
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // UserRole → Role (N - 1)
            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles) // Role’da ICollection<UserRole> UserRoles olmalı
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
