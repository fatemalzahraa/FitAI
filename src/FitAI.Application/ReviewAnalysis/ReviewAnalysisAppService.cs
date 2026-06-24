using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitAI.ReviewAnalysis;

public class ReviewAnalysisAppService : ApplicationService, IReviewAnalysisAppService
{
    private readonly ITrendyolReviewScraper _scraper;
    private static readonly HttpClient _http = new HttpClient();
    private const string PythonUrl = "http://localhost:8000";

    public ReviewAnalysisAppService(ITrendyolReviewScraper scraper)
    {
        _scraper = scraper;
    }

    public async Task<ReviewAnalysisDto> AnalyzeAsync(ReviewAnalysisInput input)
    {
        // 1. Yorumları çek
        var reviews = await _scraper.GetReviewsAsync(input.ProductUrl);

        if (reviews == null || reviews.Count == 0)
        {
            return new ReviewAnalysisDto
            {
                PositivePercent = 0,
                NeutralPercent = 0,
                NegativePercent = 0,
                Issues = new List<IssueDto>(),
                Reviews = new List<ReviewDto>()
            };
        }

        // 2. Python'a sentiment analizi yaptır
        var payload = JsonSerializer.Serialize(new
        {
            reviews = reviews.Select(r => r.Text).ToList()
        });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{PythonUrl}/yorum-analiz", content);

        if (!response.IsSuccessStatusCode)
        {
            // Python çalışmıyorsa yorumları sentiment'siz dön
            return new ReviewAnalysisDto
            {
                PositivePercent = 0,
                NeutralPercent = 100,
                NegativePercent = 0,
                Issues = new List<IssueDto>(),
                Reviews = reviews
            };
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var positivePercent = root.GetProperty("positivePercent").GetInt32();
        var neutralPercent  = root.GetProperty("neutralPercent").GetInt32();
        var negativePercent = root.GetProperty("negativePercent").GetInt32();

        // 3. Sentiment sonuçlarını ReviewDto'ya eşle
        var sentimentList = root.GetProperty("reviews").EnumerateArray().ToList();

        for (int i = 0; i < reviews.Count && i < sentimentList.Count; i++)
        {
            reviews[i].Sentiment = sentimentList[i].GetProperty("sentiment").GetString() ?? "neutral";
        }

        return new ReviewAnalysisDto
        {
            PositivePercent = positivePercent,
            NeutralPercent  = neutralPercent,
            NegativePercent = negativePercent,
            Issues = new List<IssueDto>(),
            Reviews = reviews
        };
    }
}