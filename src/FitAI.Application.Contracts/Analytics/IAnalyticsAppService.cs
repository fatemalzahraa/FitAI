using FitAI.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

public interface IAnalyticsAppService : IApplicationService
{
    // GET /api/app/analytics/store-summary?magazaId=1
    Task<DashboardSummaryDto> GetStoreSummaryAsync(int magazaId);

    // GET /api/app/analytics/sentiment-distribution?magazaId=1
    Task<List<SentimentAnalysisDto>> GetSentimentDistributionAsync(int magazaId);

    // GET /api/app/analytics/top-themes?magazaId=1&urunId=2
    Task<List<SentimentAnalysisDto>> GetTopThemesAsync(int magazaId, int? urunId = null);

    // GET /api/app/analytics/reviews?magazaId=1&duyguEtiketi=Pozitif&urunId=1&maxSayi=10
    Task<List<ReviewDto>> GetReviewsAsync(int magazaId, string? duyguEtiketi = null, int? urunId = null, int maxSayi = 10);

    // GET /api/app/analytics/dashboard-summary
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
}