using System.Threading.Tasks;
using Volo.Abp.Application.Services;


namespace FitAI.ReviewAnalysis;

public interface IReviewAnalysisAppService : IApplicationService
{
    Task<ReviewAnalysisDto> AnalyzeAsync(
        ReviewAnalysisInput input);
}