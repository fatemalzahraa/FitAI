using System.Threading.Tasks;
using FitAI.Analysis.Dtos;
using Volo.Abp.Application.Services;

namespace FitAI.Analysis;

public interface IAnalysisAppService : IApplicationService
{
  Task<FitScoreResultDto> AnalyzeAsync(
    AnalyzeProductDto input);
    
}