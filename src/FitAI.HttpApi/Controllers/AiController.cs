using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FitAI.AI;
using FitAI.Analytics;

namespace FitAI.Controllers;
[AllowAnonymous]   // ← önce bunu dene, 401 giderse sorun auth'da
//[Aut horize]           // ← EKLE: giriş yapmış kullanıcı zorunlu
[Route("api/ai")]
public class AiController : FitAIController
{
    private readonly IAiAppService _aiAppService;
    private readonly IAnalyticsAppService _analyticsAppService;

    public AiController(IAiAppService aiAppService, IAnalyticsAppService analyticsAppService)
    {
        _aiAppService = aiAppService;
        _analyticsAppService = analyticsAppService;
    }

    [HttpPost("uyum-skoru")]
    public async Task<AnalizSonucDto> UyumSkoru([FromBody] AnalizIstegiDto input)
        => await _aiAppService.AnalizYapAsync(input);

    [HttpPost("yorum-analiz/toplu")]
    public async Task<TopluYorumSonucDto> TopluYorumAnaliz([FromBody] TopluYorumIstegiDto input)
        => await _aiAppService.TopluYorumAnalizAsync(input);

    [HttpGet("dashboard-summary")]
    public async Task<DashboardSummaryDto> DashboardSummary()
        => await _analyticsAppService.GetDashboardSummaryAsync();
}