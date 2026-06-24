  import 'package:fitai_mobile/core/services/api_service.dart';
  import 'package:flutter/material.dart';
  import '../../../core/theme/app_theme.dart';
  import '../../../core/constants/app_routes.dart';
  import '../../../core/widgets/common_widgets.dart';
  import 'package:url_launcher/url_launcher.dart';

  class ProductDetailScreen extends StatefulWidget {
    const ProductDetailScreen({super.key});

    @override
    State<ProductDetailScreen> createState() => _ProductDetailScreenState();
  }

  class _ProductDetailScreenState extends State<ProductDetailScreen> {

    bool isFavoriteLoading = false;

    @override
    Widget build(BuildContext context) {

      final data =
          ModalRoute.of(context)?.settings.arguments
              as Map<String, dynamic>? ?? {};

      final productName = data["productName"] ?? "";
      final price = data["price"] ?? "";
      final platform = data["platform"] ?? "";
      final score = (data["score"] as num?)?.toInt() ?? 0;
      final recommendation = data["recommendation"] ?? "";
      final productImage = data["productImage"] ?? "";
      final productUrl = data["productUrl"] ?? "";

      return Scaffold(
        backgroundColor: AppColors.background,
        body: CustomScrollView(
          physics: const BouncingScrollPhysics(),
          slivers: [

            SliverAppBar(
              expandedHeight: 280,
              pinned: true,
              backgroundColor: Colors.white,
              leading: GestureDetector(
                onTap: () => Navigator.pop(context),
                child: Container(
                  margin: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.white.withOpacity(0.9),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(Icons.arrow_back_ios_rounded,
                      color: AppColors.textPrimary, size: 18),
                ),
              ),

              actions: [
                Container(
                  margin: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: Colors.white.withOpacity(0.9),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: IconButton(

                    icon: isFavoriteLoading
                        ? const SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Icon(
                            Icons.favorite_border_rounded,
                            color: AppColors.error,
                          ),

                    onPressed: () async {

                      setState(() => isFavoriteLoading = true);

                      try {

                        await ApiService().addFavorite(
                          productName: productName,
                          productImage: productImage,
                          platform: platform,
                          price: price,
                          score: score,
                          bodyType: "",
                        );
  Navigator.pop(context, true);
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text("Favorilere eklendi"),
                          ),
                        );

                      } catch (e) {
                        print(e);
                      }

                      setState(() => isFavoriteLoading = false);
                    },
                  ),
                ),
              ],

              flexibleSpace: FlexibleSpaceBar(
                background: Container(
                  color: AppColors.background,
                  child: productImage.isNotEmpty
                      ? Image.network(
                          productImage,
                          fit: BoxFit.cover,
                        )
                      : const Center(
                          child: Icon(
                            Icons.checkroom_rounded,
                            color: AppColors.accent,
                            size: 80,
                          ),
                        ),
                ),
              ),
            ),

            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [

                    Row(
                      children: [
                        PlatformBadge(platform: platform),
                        const SizedBox(width: 8),
                        Text(
                          price,
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.w800,
                            color: AppColors.primary,
                          ),
                        ),
                      ],
                    ),

                    const SizedBox(height: 8),

                    Text(
                      productName,
                      style: const TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.w700,
                        color: AppColors.textPrimary,
                      ),
                    ),

                    const SizedBox(height: 20),

                    GestureDetector(
                      onTap: () => Navigator.pushNamed(
    context,
    AppRoutes.fitScore,
    arguments: data,  // productUrl yerine tüm data
  ),
                      child: Container(
                        padding: const EdgeInsets.all(20),
                        decoration: BoxDecoration(
                          gradient: const LinearGradient(
                            colors: [Color(0xFFF0EDFF), Color(0xFFE8E2FF)],
                          ),
                          borderRadius: BorderRadius.circular(18),
                        ),
                        child: Row(
                          children: [

                            ScoreCircle(score: score, size: 90),

                            const SizedBox(width: 20),

                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(recommendation),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),

                    const SizedBox(height: 24),

                    Row(
                      children: [

                        Expanded(
                          child: GradientButton(
                            text: "Siteye Git",
                            onTap: () async {
                              final uri = Uri.parse(productUrl);
                              await launchUrl(
                                uri,
                                mode: LaunchMode.externalApplication,
                              );
                            },
                          ),
                        ),

                        const SizedBox(width: 10),

                        Container(
                          width: 48,
                          height: 48,
                          decoration: BoxDecoration(
                            color: AppColors.background,
                            borderRadius: BorderRadius.circular(14),
                            border: Border.all(color: AppColors.divider),
                          ),
                          child: IconButton(
                            icon: const Icon(
                              Icons.favorite_border_rounded,
                              color: AppColors.error,
                            ),
                            onPressed: () async {

                              setState(() => isFavoriteLoading = true);

                              try {

                                await ApiService().addFavorite(
                                  productName: productName,
                                  productImage: productImage,
                                  platform: platform,
                                  price: price,
                                  score: score,
                                  bodyType: "",
                                );

                                ScaffoldMessenger.of(context).showSnackBar(
                                  const SnackBar(
                                    content: Text("Favorilere eklendi"),
                                  ),
                                );

                              } catch (e) {
                                print(e);
                              }

                              setState(() => isFavoriteLoading = false);
                            },
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      );
    }
  }