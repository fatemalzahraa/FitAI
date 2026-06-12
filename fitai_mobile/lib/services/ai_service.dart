import 'dart:convert';
import 'package:http/http.dart' as http;

import '../features/fit_score/models/fit_score_result.dart';

class AIService {
  static const String baseUrl =
      "http://192.168.1.255:8000";

  static Future<FitScoreResult> analyzeReviews(
      List<String> reviews) async {

    final response = await http.post(
      Uri.parse("$baseUrl/review-analysis"),
      headers: {
        "Content-Type": "application/json",
      },
      body: jsonEncode(reviews),
    );

    if (response.statusCode == 200) {
      return FitScoreResult.fromJson(
        jsonDecode(response.body),
      );
    }

    throw Exception("AI analizi başarısız");
  }
}

