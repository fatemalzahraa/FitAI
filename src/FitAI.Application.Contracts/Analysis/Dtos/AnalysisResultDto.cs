using System.Collections.Generic;
namespace FitAI.Analysis.Dtos;

public class AnalysisResultDto
{
    public string ProductName { get; set; }

    public double Score { get; set; }

    public string Recommendation { get; set; }

    public string ProductImage { get; set; }

    public string Platform { get; set; }

    public string Price { get; set; }

    public string ProductUrl { get; set; }

    public string RiskLevel { get; set; }   // Low / Medium / High
    public string BodyMatch { get; set; }    // Armut / Dikdörtgen vs
    public List<string> Tips { get; set; }   // AI önerileri
}