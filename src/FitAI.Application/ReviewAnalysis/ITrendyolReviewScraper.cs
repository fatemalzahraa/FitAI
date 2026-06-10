using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAI.ReviewAnalysis;

public interface ITrendyolReviewScraper
{
    Task<List<ReviewDto>> GetReviewsAsync(string url);
}