using Eticaret.Core.Entities;
using Eticaret.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // Migration ve Tasarım Zamanı Araçları için
        public DatabaseContext()
        {
        }

        // DbSet Tanımları
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Arazi> Araziler { get; set; }
        public DbSet<ChatLog> ChatLogs { get; set; }
        public DbSet<ProductAgricultureProfile> ProductAgricultureProfiles { get; set; }
        public DbSet<ChatOption> ChatOptions { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1️⃣ Decimal Hassasiyet Yapılandırmaları (Uyarılardan kurtulmak için)
            modelBuilder.Entity<Arazi>().Property(x => x.Donum).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);

            // Yeni tablodaki ondalık alanlar
            modelBuilder.Entity<ProductAgricultureProfile>().Property(p => p.PhMin).HasPrecision(18, 2);
            modelBuilder.Entity<ProductAgricultureProfile>().Property(p => p.PhMax).HasPrecision(18, 2);

            // 2️⃣ Cascade Path Hatalarını Önleme (SqlException 1785 Çözümü)
            // ProductAgricultureProfile hem Product hem Category'ye bağlı olduğu için silme davranışını 'NoAction' yapıyoruz.
            modelBuilder.Entity<ProductAgricultureProfile>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductAgricultureProfile>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3️⃣ Mevcut Kimlik (Identity) İlişkileri
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4️⃣ Diğer Konfigürasyonlar
            modelBuilder.ApplyConfiguration(new Configurations.OrderItemConfiguration());
        }
    }
}