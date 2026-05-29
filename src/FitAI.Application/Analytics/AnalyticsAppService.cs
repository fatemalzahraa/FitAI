using FitAI.Domain.Ai;
using FitAI.Domain.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
namespace FitAI.Analytics;

//[Authorize] 
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
public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
{
    var toplamUrun = await _urunRepository.CountAsync(u => !u.SilindiMi);
    var toplamYorum = await _yorumRepository.CountAsync();
    var islenenYorum = await _nlpBulgusuRepository.CountAsync();

    return new DashboardSummaryDto
    {
        ToplamUrunSayisi = toplamUrun,
        ToplamYorumSayisi = toplamYorum,
        MagazaPuanOrtalamasi = 0, // İsterseniz hesaplatın
        IslenmeyiBekleyenYorumlar = toplamYorum - islenenYorum
    };
}

public async Task<List<ReviewDto>> GetReviewsAsync(int magazaId, string? duyguEtiketi = null, int? urunId = null, int maxSayi = 10)
{
    var yorumlarQuery = await _yorumRepository.GetQueryableAsync();
    var urunlerQuery = await _urunRepository.GetQueryableAsync();

    var query = from y in yorumlarQuery
                join u in urunlerQuery on y.UrunId equals u.Id
                where u.MagazaId == magazaId && !u.SilindiMi
                select y;

    if (!string.IsNullOrEmpty(duyguEtiketi))
        query = query.Where(y => y.Duygu == duyguEtiketi);

    if (urunId.HasValue)
        query = query.Where(y => y.UrunId == urunId.Value);

    var yorumlar = await query
        .OrderByDescending(y => y.CreationTime)
        .Take(maxSayi)
        .ToListAsync();

    // Mapping: Domain entity -> ReviewDto
    return yorumlar.Select(y => new ReviewDto
    {
        Id = y.Id,
        Kullanici = y.KullaniciAdi ?? "Anonim",
        Yildiz = y.Puan ?? 0,
        Metin = y.YorumMetni,
        DuyguEtiketi = y.Duygu ?? "Belirsiz",
        DuyguSkoru = y.GuvenSkoru ?? 0,
        Tema = y.Tema ?? "genel",
        Zaman = y.CreationTime
    }).ToList();
}
    public async Task<DashboardSummaryDto> GetStoreSummaryAsync(int magazaId)
    {
        var urunlerQuery = await _urunRepository.GetQueryableAsync();
        var yorumlarQuery = await _yorumRepository.GetQueryableAsync();

        var magazaUrunQuery = urunlerQuery.Where(u => u.MagazaId == magazaId && !u.SilindiMi);
        var magazaUrunIdsList = magazaUrunQuery.Select(u => u.Id).ToList();

        var magazaYorumlariQuery = yorumlarQuery.Where(y => urunlerQuery.Any(u => u.Id == y.UrunId && u.MagazaId == magazaId && !u.SilindiMi));

        var toplamYorum = magazaYorumlariQuery.Count();
        double puanOrtalamasi = 0;

        if (toplamYorum > 0)
        {
            var puanlar = magazaYorumlariQuery.Where(y => y.Puan != null).Select(y => (double)y.Puan!).ToList();
            if (puanlar.Any())
            {
                puanOrtalamasi = Math.Round(puanlar.Average(), 1);
            }
        }

        return new DashboardSummaryDto
        {
            ToplamUrunSayisi = magazaUrunIdsList.Count,
            ToplamYorumSayisi = toplamYorum,
            MagazaPuanOrtalamasi = puanOrtalamasi,
            IslenmeyiBekleyenYorumlar = magazaYorumlariQuery.Count(y => !y.NlpIslendi)
        };
    }

    public async Task<List<SentimentAnalysisDto>> GetSentimentDistributionAsync(int magazaId)
    {
        var bulgular = await _nlpBulgusuRepository.GetListAsync(b => b.MagazaId == magazaId);
        var toplam = bulgular.Count;

        if (toplam == 0)
        {
            return new List<SentimentAnalysisDto>
            {
                new SentimentAnalysisDto { Etiket = "Pozitif", Sayi = 0, Yuzde = 0 },
                new SentimentAnalysisDto { Etiket = "Negatif", Sayi = 0, Yuzde = 0 }
            };
        }

        return bulgular
            .GroupBy(b => b.DuyguEtiketi)
            .Select(g => new SentimentAnalysisDto
            {
                Etiket = g.Key ?? "Belirsiz",
                Sayi = g.Count(),
                Yuzde = Math.Round((double)g.Count() / toplam * 100, 1)
            })
            .OrderByDescending(x => x.Sayi)
            .ToList();
    }

    public async Task<List<SentimentAnalysisDto>> GetTopThemesAsync(int magazaId)
    {
        var bulgular = await _nlpBulgusuRepository.GetListAsync(b => b.MagazaId == magazaId);
        var toplam = bulgular.Count;

        if (toplam == 0)
        {
            return new List<SentimentAnalysisDto>();
        }

        return bulgular
            .GroupBy(b => b.Tema)
            .Select(g => new SentimentAnalysisDto
            {
                Etiket = g.Key ?? "Genel",
                Sayi = g.Count(),
                Yuzde = Math.Round((double)g.Count() / toplam * 100, 1)
            })
            .OrderByDescending(x => x.Sayi)
            .Take(5) 
            .ToList();
    }
}