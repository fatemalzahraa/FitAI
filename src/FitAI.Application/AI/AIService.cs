using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitAI.AI;

public class AIService
{
    private readonly HttpClient _httpClient;

    public AIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AnalyzeReviewsAsync(List<string> reviews)
    {
        var json = JsonSerializer.Serialize(reviews);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "http://127.0.0.1:8000/review-analysis",
            content
        );

        return await response.Content.ReadAsStringAsync();
    }
}