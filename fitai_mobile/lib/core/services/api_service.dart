import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {

  static final Dio dio = Dio(
    BaseOptions(
      baseUrl: 'http://192.168.0.113:5000',
      headers: {
        'Content-Type': 'application/json',
      },
    ),
  )..interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final prefs = await SharedPreferences.getInstance();
          final token = prefs.getString('token');

          print("TOKEN FROM STORAGE: $token");

          if (token != null) {
            options.headers['Authorization'] = "Bearer $token";
          }

          return handler.next(options);
        },
      ),
    );

  // ================= PROFILE =================

  Future<Response> getMyProfile() {
    return dio.get("/api/profile/my-profile");
  }

  Future<Response> updateBodyType(String bodyType) {
    return dio.put(
      "/api/profile/body-type",
      data: {
        "bodyType": bodyType,
      },
    );
  }

  // ================= FAVORITES =================

  Future<Response> getFavorites() {
    return dio.get("/api/favorites");
  }

  Future<Response> addFavorite({
    required String productName,
    required String productImage,
    required String platform,
    required String price,
    required int score,
    required String bodyType,
  }) {
    return dio.post(
      "/api/favorites",
      data: {
        "productName": productName,
        "productImage": productImage,
        "platform": platform,
        "price": price,
        "score": score,
        "bodyType": bodyType,
      },
    );
  }

  Future<Response> deleteFavorite(int id) {
    return dio.delete("/api/favorites/$id");
  }

  // ================= ANALYSIS =================

  Future<Response> analyzeProduct(String productUrl) {
    return dio.post(
      "/api/app/analysis/analyze",
      data: {
        "productUrl": productUrl,
      },
    );
  }

  Future<Response> getFitScore(String productUrl) {
    return dio.post(
      "/api/app/analysis/analyze",
      data: {
        "productUrl": productUrl,
      },
    );
  }

  // 🔥 EKLENDİ (SENİN EKSİK OLAN KISIM)

  Future<Response> getMyAnalyses() {
    return dio.get("/api/analysis");
  }
  Future<Response> analyzeReviews(String productUrl) {
  return dio.post(
    "/api/app/review-analysis/analyze",
    data: {
      "productUrl": productUrl,
    },
  );
}
}