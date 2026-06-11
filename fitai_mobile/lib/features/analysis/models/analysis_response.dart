class AnalysisResponse {

  final String productName;
  final int score;
  final String recommendation;
  final String productImage;
  final String platform;
  final String price;

  AnalysisResponse({
    required this.productName,
    required this.score,
    required this.recommendation,
    required this.productImage,
    required this.platform,
    required this.price,
  });

  factory AnalysisResponse.fromJson(Map<String, dynamic> json) {

    return AnalysisResponse(
      productName: json["productName"],
      score: json["score"],
      recommendation: json["recommendation"],
      productImage: json["productImage"],
      platform: json["platform"],
      price: json["price"],
    );
  }
}