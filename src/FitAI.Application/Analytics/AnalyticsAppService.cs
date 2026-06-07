using FitAI.Analytics;
using FitAI.Domain.Ai;
using FitAI.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FitAI.Analytics;

// [Authorize]
public class AnalyticsAppService : FitAIAppService, IAnalyticsAppService
{
    private readonly IRepository<Urun, int> _urunRepository;
    private readonly IRepository<Yorum, int> _yorumRepository;
    private readonly IRepository<NlpBulgusu, int> _nlpBulgusuRepository;

    public AnalyticsAppService(
        IRepository<Urun, int> urunRepository,
        IRepository<Yorum, int> yorumRepository,
        IRepository<NlpBulgusu, int> nlpBulgusuRepository)
    {
        _urunRepository = urunRepository;
        _yorumRepository = yorumRepository;
        _nlpBulgusuRepository = nlpBulgusuRepository;
    }

    // ─── Dashboard Summary (mağaza filtresi olmaksızın) ──────

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var toplamUrun    = await _urunRepository.CountAsync(u => !u.SilindiMi);
        var toplamYorum   = await _yorumRepository.CountAsync();
        var islenenYorum  = await _nlpBulgusuRepository.CountAsync();

        return new DashboardSummaryDto
        {
            ToplamUrunSayisi          = toplamUrun,
            ToplamYorumSayisi         = toplamYorum,
            MagazaPuanOrtalamasi      = 0,
            IslenmeyiBekleyenYorumlar = toplamYorum - islenenYorum
        };
    }

    // ─── Store Summary ───────────────────────────────────────
    // GET /api/app/analytics/store-summary?magazaId=1

    public async Task<DashboardSummaryDto> GetStoreSummaryAsync(int magazaId)
    {
        var urunlerQuery   = await _urunRepository.GetQueryableAsync();
        var yorumlarQuery  = await _yorumRepository.GetQueryableAsync();

        var magazaUrunIdleri = urunlerQuery
            .Where(u => u.MagazaId == magazaId && !u.SilindiMi)
            .Select(u => u.Id)
            .ToList();

        var magazaYorumlari = yorumlarQuery
            .Where(y => magazaUrunIdleri.Contains(y.UrunId));

        var toplamYorum   = magazaYorumlari.Count();
        double puanOrt    = 0;

        if (toplamYorum > 0)
        {
            var puanlar = magazaYorumlari
                .Where(y => y.Puan != null)
                .Select(y => (double)y.Puan!)
                .ToList();

            if (puanlar.Any())
                puanOrt = Math.Round(puanlar.Average(), 1);
        }

        return new DashboardSummaryDto
        {
            ToplamUrunSayisi          = magazaUrunIdleri.Count,
            ToplamYorumSayisi         = toplamYorum,
            MagazaPuanOrtalamasi      = puanOrt,
            IslenmeyiBekleyenYorumlar = magazaYorumlari.Count(y => !y.NlpIslendi)
        };
    }

    // ─── Sentiment Distribution ──────────────────────────────
    // GET /api/app/analytics/sentiment-distribution?magazaId=1

    public async Task<List<SentimentAnalysisDto>> GetSentimentDistributionAsync(int magazaId)
    {
        var bulgular = await _nlpBulgusuRepository.GetListAsync(b => b.MagazaId == magazaId);
        var toplam   = bulgular.Count;

        if (toplam == 0)
        {
            return new List<SentimentAnalysisDto>
            {
                new() { Etiket = "Pozitif",  Sayi = 0, Yuzde = 0 },
                new() { Etiket = "Negatif",  Sayi = 0, Yuzde = 0 },
                new() { Etiket = "Belirsiz", Sayi = 0, Yuzde = 0 }
            };
        }

        return bulgular
            .GroupBy(b => b.DuyguEtiketi)
            .Select(g => new SentimentAnalysisDto
            {
                Etiket = g.Key ?? "Belirsiz",
                Sayi   = g.Count(),
                Yuzde  = Math.Round((double)g.Count() / toplam * 100, 1)
            })
            .OrderByDescending(x => x.Sayi)
            .ToList();
    }

    // ─── Top Themes ──────────────────────────────────────────
    // GET /api/app/analytics/top-themes?magazaId=1
    // GET /api/app/analytics/top-themes?magazaId=1&urunId=2
    //
    // JS'de nlpOzetiniGuncelle() hem magazaId hem urunId gönderiyor;
    // urunId opsiyonel parametre olarak alınıyor.

    public async Task<List<SentimentAnalysisDto>> GetTopThemesAsync(int magazaId, int? urunId = null)
    {
        var bulgu = await _nlpBulgusuRepository.GetQueryableAsync();

        var query = bulgu.Where(b => b.MagazaId == magazaId);

        if (urunId.HasValue)
            query = query.Where(b => b.UrunId == urunId.Value);

        var liste  = query.ToList();
        var toplam = liste.Count;

        if (toplam == 0)
            return new List<SentimentAnalysisDto>();

        return liste
            .GroupBy(b => b.Tema)
            .Select(g => new SentimentAnalysisDto
            {
                Etiket = g.Key ?? "Genel",
                Sayi   = g.Count(),
                Yuzde  = Math.Round((double)g.Count() / toplam * 100, 1)
            })
            .OrderByDescending(x => x.Sayi)
            .Take(5)
            .ToList();
    }

    // ─── Reviews ─────────────────────────────────────────────
    // GET /api/app/analytics/reviews?magazaId=1&duyguEtiketi=Pozitif&urunId=1&maxSayi=10

    public async Task<List<ReviewDto>> GetReviewsAsync(
        int     magazaId,
        string? duyguEtiketi = null,
        int?    urunId       = null,
        int     maxSayi      = 10)
    {
        var urunlerQuery  = await _urunRepository.GetQueryableAsync();
        var yorumlarQuery = await _yorumRepository.GetQueryableAsync();

        // Mağazaya ait aktif ürünlerin ID'lerini al
        var magazaUrunIdleri = urunlerQuery
            .Where(u => u.MagazaId == magazaId && !u.SilindiMi)
            .Select(u => u.Id);

        var query = yorumlarQuery
            .Where(y => magazaUrunIdleri.Contains(y.UrunId));

        if (!string.IsNullOrWhiteSpace(duyguEtiketi))
            query = query.Where(y => y.Duygu == duyguEtiketi);

        if (urunId.HasValue)
            query = query.Where(y => y.UrunId == urunId.Value);

        var yorumlar = await query
            .OrderByDescending(y => y.CreationTime)
            .Take(maxSayi)
            .ToListAsync();

        return yorumlar.Select(y => new ReviewDto
        {
            Id           = y.Id,
            Kullanici    = y.KullaniciAdi ?? "Anonim",
            Yildiz       = y.Puan        ?? 0,
            Metin        = y.YorumMetni,
            DuyguEtiketi = y.Duygu       ?? "Belirsiz",
            DuyguSkoru   = y.GuvenSkoru  ?? 0,
            Tema         = y.Tema        ?? "genel",
            Zaman        = y.CreationTime
        }).ToList();
    }
}