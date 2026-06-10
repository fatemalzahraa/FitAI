import 'package:flutter/material.dart';

import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';
import '../../../core/services/api_service.dart';

class FavoritesScreen extends StatefulWidget {
  const FavoritesScreen({super.key});

  @override
  State<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends State<FavoritesScreen> {

  final ApiService _api = ApiService();

  List favorites = [];

  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    loadFavorites();
  }
@override
void didChangeDependencies() {
  super.didChangeDependencies();
  Future.microtask(() => loadFavorites());
}
  Future<void> loadFavorites() async {
  try {
    final res = await _api.getFavorites();

    print("FAVORITES RESPONSE:");
    print(res.data);

    final data = res.data;

    setState(() {
      favorites = data is List ? data : data["items"] ?? [];
      print("FAVORITES LENGTH: ${favorites.length}");
      isLoading = false;
    });
  } catch (e) {
    print(e);
    setState(() {
      isLoading = false;
    });
  }
}

  Future<void> deleteFavorite(int id) async {

    try {

      await _api.deleteFavorite(id);

      loadFavorites();

    } catch (e) {

      print(e);
    }
  }

  @override
  Widget build(BuildContext context) {

    return Scaffold(
      
      backgroundColor: Colors.transparent,
      body: Container(
        decoration: const BoxDecoration(
          gradient: AppColors.bgGradient,
        ),
        child: SafeArea(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [

              Padding(
                padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [

                    const Text(
                      'Favorilerim',
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w800,
                        color: AppColors.textPrimary,
                        letterSpacing: -0.5,
                      ),
                    ),

                    Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 10,
                        vertical: 5,
                      ),
                      decoration: BoxDecoration(
                        color: AppColors.primary.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        '${favorites.length} ürün',
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w600,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 16),

              if (isLoading)
                const Expanded(
                  child: Center(
                    child: CircularProgressIndicator(),
                  ),
                )
              else
                Expanded(
                  child: favorites.isEmpty
                      ? _EmptyFavorites()
                      : ListView.builder(
                          physics: const BouncingScrollPhysics(),
                          padding: const EdgeInsets.symmetric(horizontal: 20),
                          itemCount: favorites.length,
                          itemBuilder: (context, i) {

                            final item = favorites[i];
                            

  print(item);

                            return _FavoriteCard(

                              id: item["id"],

                              name: item['productName'] ?? "",

                              platform: item['platform'] ?? "",

                              score: item['score'] ?? 0,

                              bodyType: item['bodyType'] ?? "",

                              price: item['price'] ?? "",

                              image: item['productImage'] ?? "",

                              onTap: () {

                                Navigator.pushNamed(
                                  context,
                                  AppRoutes.productDetail,
                                );
                              },

                              onDelete: () async {

                                await deleteFavorite(item["id"]);
                              },
                            );
                          },
                        ),
                ),
                
            ],
          ),
        ),
      ),
    );
  }
}

class _FavoriteCard extends StatelessWidget {

  final int id;

  final String name;
  final String platform;
  final int score;
  final String bodyType;
  final String price;
  final String image;

  final VoidCallback onTap;
  final VoidCallback onDelete;

  const _FavoriteCard({
    required this.id,
    required this.name,
    required this.platform,
    required this.score,
    required this.bodyType,
    required this.price,
    required this.image,
    required this.onTap,
    required this.onDelete,
  });

  @override
  Widget build(BuildContext context) {

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.divider),
      ),
      child: Row(
        children: [

          ClipRRect(
            borderRadius: BorderRadius.circular(10),

            child: image.startsWith("http")

                ? Image.network(
                    image,
                    width: 68,
                    height: 68,
                    fit: BoxFit.cover,
                  )

                : Image.asset(
                    image,
                    width: 68,
                    height: 68,
                    fit: BoxFit.cover,
                  ),
          ),

          const SizedBox(width: 12),

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [

                Row(
                  children: [

                    PlatformBadge(platform: platform),

                    const Spacer(),

                    Text(
                      price,
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w700,
                        color: AppColors.primary,
                      ),
                    ),
                  ],
                ),

                const SizedBox(height: 5),

                Text(
                  name,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                    color: AppColors.textPrimary,
                    height: 1.3,
                  ),
                ),

                const SizedBox(height: 6),

                Row(
                  children: [

                    ScoreCircle(
                      score: score,
                      size: 36,
                    ),

                    const SizedBox(width: 8),

                    

                    const Spacer(),

                    IconButton(
                      icon: const Icon(
                        Icons.favorite_rounded,
                        color: AppColors.error,
                        size: 20,
                      ),

                      onPressed: onDelete,

                      padding: EdgeInsets.zero,
                      constraints: const BoxConstraints(),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _EmptyFavorites extends StatelessWidget {

  @override
  Widget build(BuildContext context) {

    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [

          Container(
            width: 80,
            height: 80,
            decoration: BoxDecoration(
              color: AppColors.primary.withOpacity(0.1),
              shape: BoxShape.circle,
            ),
            child: const Icon(
              Icons.favorite_border_rounded,
              color: AppColors.primary,
              size: 36,
            ),
          ),

          const SizedBox(height: 16),

          const Text(
            'Henüz favori yok',
            style: TextStyle(
              fontSize: 17,
              fontWeight: FontWeight.w700,
              color: AppColors.textPrimary,
            ),
          ),

          const SizedBox(height: 6),

          const Text(
            'Beğendiğin ürünleri favorilere ekle',
            style: TextStyle(
              fontSize: 13,
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }
}