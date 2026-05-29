using FitAI.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

public interface IAnalyticsAppService : IApplicationService
{
   
       // Mevcut metotlar
    Task<DashboardSummaryDto> GetStoreSummaryAsync(int magazaId);
    Task<List<SentimentAnalysisDto>> GetSentimentDistributionAsync(int magazaId);
    Task<List<SentimentAnalysisDto>> GetTopThemesAsync(int magazaId);

    // Yeni eklenen metotlar
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<List<ReviewDto>> GetReviewsAsync(int magazaId, string? duyguEtiketi = null, int? urunId = null, int maxSayi = 10);
   
    // GET /api/app/analytics/store-summary?magazaId=1

    // GET /api/app/analytics/sentiment-distribution?magazaId=1

    // GET /api/app/analytics/top-themes?magazaId=1

    // GET /api/app/analytics/reviews?magazaId=1&duyguEtiketi=Pozitif&urunId=1&maxSayi=10
}