using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FitAI.Analysis.Dtos;
using Volo.Abp.Application.Services;

namespace FitAI.Analysis;

public class AnalysisAppService
    : ApplicationService,
      IAnalysisAppService
{
    public async Task<FitScoreResultDto> AnalyzeAsync(
        AnalyzeProductDto input)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0");

        var html = await httpClient.GetStringAsync(input.ProductUrl);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var titleNode = doc.DocumentNode
            .SelectSingleNode("//title");

        string productName =
            titleNode?.InnerText ?? "Ürün";

        var imageNode = doc.DocumentNode.SelectSingleNode(
            "//meta[@property='og:image']");

        string productImage =
            imageNode?.GetAttributeValue("content", "") ?? "";

        return new FitScoreResultDto
        {
            ProductName = productName,

            Score = 85,

            Recommendation = "Vücut tipine uygun",

            RiskLevel = "HIGH",

            SizeRecommendation = "Büyük Al",

            Details = new List<ScoreDetailDto>
            {
                new ScoreDetailDto
                {
                    Label = "Omuz Genişliği",
                    Score = 88
                },

                new ScoreDetailDto
                {
                    Label = "Göğüs Çevresi",
                    Score = 92
                },

                new ScoreDetailDto
                {
                    Label = "Bel Çevresi",
                    Score = 75
                },

                new ScoreDetailDto
                {
                    Label = "Kumaş Esnekliği",
                    Score = 80
                }
            },

            AiSuggestions = new List<string>
            {
                "Bu ürün armut vücut tipine uygundur.",
                "Kumaş yapısı serttir.",
                "Bir beden büyük alman önerilir."
            },

            ProductImage = productImage,

            Platform = "Trendyol",

            Price = "",
            ProductUrl = input.ProductUrl,
        };
    }
}