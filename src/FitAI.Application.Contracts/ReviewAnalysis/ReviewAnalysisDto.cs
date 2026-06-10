using System.Collections.Generic;

namespace FitAI.ReviewAnalysis;

public class ReviewAnalysisDto
{
    public int PositivePercent { get; set; }

    public int NeutralPercent { get; set; }

    public int NegativePercent { get; set; }

    public List<IssueDto> Issues { get; set; } = [];

    public List<ReviewDto> Reviews { get; set; } = [];
}