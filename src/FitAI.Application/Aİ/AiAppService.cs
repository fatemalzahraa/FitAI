using FitAI.Ai;
using FitAI.Domain.Ai;
using FitAI.Domain.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FitAI.Ai;

//[Authorize] 
public class AiAppService : FitAIAppService, IAiAppService
{
    private readonly IRepository<Yorum, int> _yorumRepository;
    private readonly IRepository<NlpBulgusu, int> _nlpBulgusuRepository;

    public AiAppService(
        IRepository<Yorum, int> yorumRepository,
        IRepository<NlpBulgusu, int> nlpBulgusuRepository)
    {
        _yorumRepository = yorumRepository;
        _nlpBulgusuRepository = nlpBulgusuRepository;
    }

    public async Task<NlpResultDto> AnalyzeCommentAsync(string yorumMetni)
    {
        if (string.IsNullOrWhiteSpace(yorumMetni))
        {
            return new NlpResultDto
            {
                Duygu = "Nötr",
                GuvenSkoru = 1.00,
                TespitEdilenKonu = "Bilinmiyor"
            };
        }

        var metin = yorumMetni.ToLower();

        var result = new NlpResultDto
        {
            Duygu = metin.Contains("kötü") || metin.Contains("dar") ? "Negatif" : "Pozitif",
            GuvenSkoru = 0.85,
            TespitEdilenKonu = metin.Contains("kargo") ? "Lojistik" :
                               metin.Contains("fiyat") ? "Maliyet" : "Ürün Kalitesi"
        };

        return await Task.FromResult(result);
    }

    public async Task ProcessPendingCommentsAsync(int magazaId)
    {
        var pendingComments = await _yorumRepository.GetListAsync(y => !y.NlpIslendi && y.MagazaId == magazaId);

        foreach (var yorum in pendingComments)
        {
            try
            {
                var analizSonucu = await AnalyzeCommentAsync(yorum.YorumMetni);

                await _nlpBulgusuRepository.InsertAsync(new NlpBulgusu
                {
                    UrunId = yorum.UrunId,
                    MagazaId = magazaId,
                    Tema = analizSonucu.TespitEdilenKonu,
                    DuyguEtiketi = analizSonucu.Duygu,
                    DuyguSkoru = (decimal)analizSonucu.GuvenSkoru,
                    TekrarSayisi = 1,
                    Durum = "Acik",
                    OneriMetni = analizSonucu.Duygu == "Negatif" ? "Bu konuyla ilgili müşteri geri bildirimlerini inceleyin." : null
                }, autoSave: true);

                yorum.NlpIslendi = true;
                await _yorumRepository.UpdateAsync(yorum);
            }
            catch (Exception ex)
            {
                // HATA YAKALAMA: Döngü esnasında bir yorumda sorun çıkarsa sistem kilitlenmez, log atıp sıradakine geçer.
                Logger.LogError(ex, $"Yorum ID: {yorum.Id} işlenirken bir hata oluştu. Bir sonraki yoruma geçiliyor.");
                continue;
            }
        }
    }
}