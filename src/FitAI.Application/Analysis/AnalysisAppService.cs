using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitAI.Analysis.Dtos;
using HtmlAgilityPack;
using Volo.Abp.Application.Services;

namespace FitAI.Analysis;

public class AnalysisAppService : ApplicationService, IAnalysisAppService
{
    private static readonly HttpClient _http = new HttpClient();
    private const string PythonUrl = "http://localhost:8000";

    public async Task<FitScoreResultDto> AnalyzeAsync(AnalyzeProductDto input)
    {
        // ── 1. HTML scraping ile ürün bilgilerini çek ──
        _http.DefaultRequestHeaders.Remove("User-Agent");
        _http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        var html = await _http.GetStringAsync(input.ProductUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        string productName = titleNode?.InnerText?.Trim() ?? "Ürün";

        var imageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
        string productImage = imageNode?.GetAttributeValue("content", "") ?? "";

        var priceNode = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'prc-dsc')]")
                     ?? doc.DocumentNode.SelectSingleNode("//*[contains(@class,'product-price')]");
        string price = priceNode?.InnerText?.Trim() ?? "";

        string platform = input.ProductUrl.Contains("trendyol") ? "Trendyol" : "Hepsiburada";

        // ── 2. Python AI servisine gönder ──
        var payload = JsonSerializer.Serialize(new
        {
            productUrl = input.ProductUrl,
            vucut_tipi = input.BodyType ?? "Armut"
        });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var aiResponse = await _http.PostAsync($"{PythonUrl}/url-analiz", content);

        // ── 3. AI yanıtını oku ──
        if (!aiResponse.IsSuccessStatusCode)
        {
            // AI çalışmazsa hardcode fallback
            return new FitScoreResultDto
            {
                ProductName = productName,
                ProductImage = productImage,
                Price = price,
                Platform = platform,
                ProductUrl = input.ProductUrl,
                Score = 70,
                Recommendation = "Vücut tipine uygun",
                RiskLevel = "Orta",
                SizeRecommendation = "Normal Beden",
                Details = new List<ScoreDetailDto>
                {
                    new ScoreDetailDto { Label = "Omuz Genişliği", Score = 88 },
                    new ScoreDetailDto { Label = "Göğüs Çevresi",  Score = 92 },
                    new ScoreDetailDto { Label = "Bel Çevresi",     Score = 75 },
                    new ScoreDetailDto { Label = "Kumaş Esnekliği", Score = 80 },
                },
                AiSuggestions = new List<string>
                {
                    "AI servisi şu an yanıt vermiyor.",
                    "Lütfen daha sonra tekrar deneyin.",
                }
            };
        }

        var json = await aiResponse.Content.ReadAsStringAsync();
        var aiResult = JsonSerializer.Deserialize<FitScoreResultDto>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // ── 4. Scraping'den gelen bilgileri AI sonucuna ekle ──
        // Python scraping bazen boş dönebilir, C# tarafını öncelikli kullan
        if (string.IsNullOrEmpty(aiResult.ProductName) || aiResult.ProductName == "Ürün")
            aiResult.ProductName = productName;

        if (string.IsNullOrEmpty(aiResult.ProductImage))
            aiResult.ProductImage = productImage;

        if (string.IsNullOrEmpty(aiResult.Price))
            aiResult.Price = price;

        if (string.IsNullOrEmpty(aiResult.Platform))
            aiResult.Platform = platform;

        aiResult.ProductUrl = input.ProductUrl;

        return aiResult;
    }
}