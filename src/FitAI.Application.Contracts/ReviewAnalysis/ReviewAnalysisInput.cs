using Volo.Abp.Application.Dtos;

namespace FitAI.ReviewAnalysis;

public class ReviewAnalysisInput : EntityDto
{
    public string ProductUrl { get; set; }
}