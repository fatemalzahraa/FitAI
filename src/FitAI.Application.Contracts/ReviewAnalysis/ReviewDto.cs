namespace FitAI.ReviewAnalysis;

public class ReviewDto
{
    public string Text { get; set; }

    public string Sentiment { get; set; }
    // positive
    // neutral
    // negative

    public string? Issue { get; set; }

    public string Time { get; set; }
}