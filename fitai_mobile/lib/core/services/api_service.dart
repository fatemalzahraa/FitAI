import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {

  static final Dio dio = Dio(

    BaseOptions(

      baseUrl: 'http://192.168.0.115:5000',

      headers: {
        'Content-Type': 'application/json',
      },

    ),

  )

    ..interceptors.add(

      InterceptorsWrapper(

  onRequest: (options, handler) async {

  final prefs = await SharedPreferences.getInstance();

  final token = prefs.getString('token');

  print("TOKEN FROM STORAGE: $token");

  if (token != null) {
    options.headers['Authorization'] = "Bearer $token";
  }

  print("HEADERS: ${options.headers}");

  return handler.next(options);
},

      ),

    );
Future<Response> getMyProfile() {
  return dio.get("/api/profile/my-profile");
}

Future<Response> updateBodyType(String bodyType) {
  return dio.put(
    "/api/account/body-type",
    data: {"bodyType": bodyType},  // DTO'ya uygun
  );
}
  // FAVORITES

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
}