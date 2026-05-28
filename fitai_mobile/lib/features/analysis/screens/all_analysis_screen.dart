import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/widgets/common_widgets.dart';

class AllAnalysisScreen extends StatelessWidget {
  const AllAnalysisScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Tüm Analizler'),
      ),
      body: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          AnalysisCard(
            productName: 'Kudos Açık Mavi Denim Gömlek',
            platform: 'Trendyol',
            bodyType: 'Armut',
            score: 73,
            timeAgo: '12 saat önce',
            image: 'assets/images/p1.jpeg',
            onTap: () {},
          ),
          AnalysisCard(
            productName: 'Yeşil Kaşe Kazak',
            platform: 'Trendyol',
            bodyType: 'Armut',
            score: 94,
            timeAgo: '3 saat önce',
            image:'assets/images/p2.jpeg',
            onTap: () {},
          ),
        ],
      ),
    );
  }
}