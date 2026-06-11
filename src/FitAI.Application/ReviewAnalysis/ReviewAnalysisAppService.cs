using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitAI.ReviewAnalysis;

public class ReviewAnalysisAppService :
    ApplicationService,
    IReviewAnalysisAppService
{
    private readonly ITrendyolReviewScraper _scraper;

    public ReviewAnalysisAppService(ITrendyolReviewScraper scraper)
    {
        _scraper = scraper;
    }

    public async Task<ReviewAnalysisDto> AnalyzeAsync(
        ReviewAnalysisInput input)
    {
        var reviews = await _scraper.GetReviewsAsync(input.ProductUrl);

        return new ReviewAnalysisDto
        {
            PositivePercent = 68,
            NeutralPercent = 18,
            NegativePercent = 14,

            Issues = new List<IssueDto>
            {
                new()
                {
                    Label = "Beden",
                    Count = 12
                },
                new()
                {
                    Label = "Kumaş",
                    Count = 8
                }
            },

            Reviews = reviews
        };
    }
}