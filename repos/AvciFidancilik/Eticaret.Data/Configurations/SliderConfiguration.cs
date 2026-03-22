using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Eticaret.Core.Entities;

namespace Eticaret.Data.Configurations
{
    public class SliderConfiguration : IEntityTypeConfiguration<Slider>
    {
        public void Configure(EntityTypeBuilder<Slider> builder)
        {
            // Tablo adı
            builder.ToTable("Sliders");

            // Primary Key
            builder.HasKey(s => s.Id);

            // Alan özellikleri
            builder.Property(s => s.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(s => s.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.SubTitle)
                   .HasMaxLength(300);

            builder.Property(s => s.IsActive)
                   .HasDefaultValue(true); // Varsayılan aktif

           
        }
    }
}
