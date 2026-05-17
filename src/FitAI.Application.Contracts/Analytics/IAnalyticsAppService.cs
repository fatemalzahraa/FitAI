using FitAI.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

public interface IAnalyticsAppService : IApplicationService
{
    Task<DashboardSummaryDto> GetStoreSummaryAsync(int magazaId);

    Task<List<SentimentAnalysisDto>> GetSentimentDistributionAsync(int magazaId);

    Task<List<SentimentAnalysisDto>> GetTopThemesAsync(int magazaId);
}