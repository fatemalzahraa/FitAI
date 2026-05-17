using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FitAI.Ai;

public interface IAiAppService : IApplicationService
{
    Task<NlpResultDto> AnalyzeCommentAsync(string yorumMetni);

    Task ProcessPendingCommentsAsync(int magazaId);
}