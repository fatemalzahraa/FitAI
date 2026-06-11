class FitScoreResult {
  final double score;
  final String recommendation;
  final String riskLevel;
  final String sizeRecommendation;
  final List<ScoreDetail> details;
  final List<String> aiSuggestions;

  FitScoreResult({
    required this.score,
    required this.recommendation,
    required this.riskLevel,
    required this.sizeRecommendation,
    required this.details,
    required this.aiSuggestions,
  });

  factory FitScoreResult.fromJson(Map<String, dynamic> json) {
    return FitScoreResult(
      score: json["score"],
      recommendation: json["recommendation"],
      riskLevel: json["riskLevel"],
      sizeRecommendation: json["sizeRecommendation"],
      details: (json["details"] as List)
          .map((e) => ScoreDetail.fromJson(e))
          .toList(),
      aiSuggestions: List<String>.from(json["aiSuggestions"]),
    );
  }
}

class ScoreDetail {
  final String label;
  final int score;

  ScoreDetail({
    required this.label,
    required this.score,
  });

  factory ScoreDetail.fromJson(Map<String, dynamic> json) {
    return ScoreDetail(
      label: json["label"],
      score: json["score"],
    );
  }
}