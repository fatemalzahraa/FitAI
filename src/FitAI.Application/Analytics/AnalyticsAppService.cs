using FitAI.Domain.Ai;
using FitAI.Domain.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

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