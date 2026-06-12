using Microsoft.AspNetCore.Mvc;
using FitAI.AI;
using System.Threading.Tasks;
namespace FitAI.Controllers;
using System.Collections.Generic;

[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly AIService _aiService;

    public AIController(AIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("analyze")]
public async Task<string> Analyze(
    [FromBody] List<string> reviews)
{
    return await _aiService.AnalyzeReviewsAsync(reviews);
}
}