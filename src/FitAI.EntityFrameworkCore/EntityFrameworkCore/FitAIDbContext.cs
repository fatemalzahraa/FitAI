using FitAI.Domain.Ai;
using FitAI.Domain.Analytics;
using FitAI.Domain.Commerce;
using FitAI.Domain.Integration;
using FitAI.Domain.Notifications;
using FitAI.Domain.Onboarding;
using FitAI.Domain.Products;
using FitAI.Domain.Scoring;
using FitAI.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;

namespace FitAI.EntityFrameworkCore;

[ConnectionStringName("Default")]
[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
public class FitAIDbContext :
    AbpDbContext<FitAIDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    // ================= DOMAIN =================

    public DbSet<Magaza> Magazalar => Set<Magaza>();
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<KullaniciProfil> KullaniciProfilleri => Set<KullaniciProfil>();

    public DbSet<Urun> Urunler => Set<Urun>();
    public DbSet<Yorum> Yorumlar => Set<Yorum>();

    public DbSet<PlatformBaglantisi> PlatformBaglantilari => Set<PlatformBaglantisi>();
    public DbSet<SenkronizasyonLog> SenkronizasyonLoglari => Set<SenkronizasyonLog>();

    public DbSet<AiTalimat> AiTalimatlar => Set<AiTalimat>();
    public DbSet<NlpBulgusu> NlpBulgulari => Set<NlpBulgusu>();

    public DbSet<Bildirim> Bildirimler => Set<Bildirim>();
    public DbSet<OnboardingAdim> OnboardingAdimlari => Set<OnboardingAdim>();

    public DbSet<KomisyonKaydi> KomisyonKayitlari => Set<KomisyonKaydi>();
    public DbSet<VucutUyumSkoru> VucutUyumSkorlari => Set<VucutUyumSkoru>();

    public DbSet<WidgetSorguLog> WidgetSorguLoglari => Set<WidgetSorguLog>();

    // ================= ABP =================

    public DbSet<IdentityUser> Users { get; set; } = null!;
    public DbSet<IdentityRole> Roles { get; set; } = null!;
    public DbSet<IdentityClaimType> ClaimTypes { get; set; } = null!;
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; } = null!;
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; } = null!;
    public DbSet<IdentityLinkUser> LinkUsers { get; set; } = null!;
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; } = null!;
    public DbSet<IdentitySession> Sessions { get; set; } = null!;

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; } = null!;

    public FitAIDbContext(DbContextOptions<FitAIDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ================= ABP MODULES =================
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureTenantManagement();

        // ================= DOMAIN REGISTRATION =================
        builder.Entity<Magaza>();
        builder.Entity<Kullanici>();
        builder.Entity<KullaniciProfil>();

        builder.Entity<Urun>();

        // --- YORUMLAR TABLOSU PERFORMANS OPTİMİZASYONU & İLİŞKİ DÜZENLEMESİ ---
builder.Entity<Yorum>(b =>
{
    b.ConfigureByConvention();

    b.HasOne(y => y.Urun)
     .WithMany()
     .HasForeignKey(y => y.UrunId)
     .OnDelete(DeleteBehavior.NoAction);

    b.HasOne(y => y.Magaza)
     .WithMany()
     .HasForeignKey(y => y.MagazaId)
     .OnDelete(DeleteBehavior.NoAction);

    b.HasIndex(x => new { x.MagazaId, x.NlpIslendi });
    b.HasIndex(x => x.UrunId);
});
        builder.Entity<PlatformBaglantisi>();
        builder.Entity<SenkronizasyonLog>();

        builder.Entity<AiTalimat>();

        // --- NLP BULGULARI TABLOSU PERFORMANS OPTİMİZASYONU ---
        builder.Entity<NlpBulgusu>(b =>
        {
            b.ConfigureByConvention();

            // Analytics servisinin GroupBy ve Where (MagazaId) sorgularını uçuracak index
            b.HasIndex(x => x.MagazaId);
            b.HasIndex(x => x.UrunId);
        });

        builder.Entity<Bildirim>();
        builder.Entity<OnboardingAdim>();

        builder.Entity<KomisyonKaydi>();
        builder.Entity<VucutUyumSkoru>();

        builder.Entity<WidgetSorguLog>();

        builder.ConfigureBackgroundJobs();
    }
}