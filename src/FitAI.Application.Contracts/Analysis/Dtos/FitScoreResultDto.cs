using System.Collections.Generic;

namespace FitAI.Analysis.Dtos;

public class FitScoreResultDto
{
    public string ProductName { get; set; }

    public double Score { get; set; }

    public string Recommendation { get; set; }

    public string RiskLevel { get; set; }

    public string SizeRecommendation { get; set; }

    public List<ScoreDetailDto> Details { get; set; }

    public List<string> AiSuggestions { get; set; }

    public string ProductImage { get; set; }

    public string Platform { get; set; }

    public string Price { get; set; }
    public string ProductUrl { get; set; }
}