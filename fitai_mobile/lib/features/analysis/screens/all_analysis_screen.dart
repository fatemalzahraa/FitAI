import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/widgets/common_widgets.dart';
import '../../../core/services/api_service.dart';

class AllAnalysisScreen extends StatefulWidget {
  const AllAnalysisScreen({super.key});

  @override
  State<AllAnalysisScreen> createState() => _AllAnalysisScreenState();
}

class _AllAnalysisScreenState extends State<AllAnalysisScreen> {

  List analyses = [];
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    loadAllAnalyses();
  }

  Future<void> loadAllAnalyses() async {
    try {
      final res = await ApiService().getMyAnalyses();

      setState(() {
        analyses = res.data["items"] ?? [];
        isLoading = false;
      });

    } catch (e) {
      print(e);
      setState(() {
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Tüm Analizler'),
      ),

      body: isLoading
          ? const Center(child: CircularProgressIndicator())

          : analyses.isEmpty
              ? const Center(
                  child: Text("Henüz analiz bulunmuyor"),
                )

              : ListView.builder(
                  padding: const EdgeInsets.all(20),
                  itemCount: analyses.length,
                  itemBuilder: (context, i) {

                    final item = analyses[i];

                    return AnalysisCard(
                      productName: item["productName"] ?? "",
                      platform: item["platform"] ?? "",
                      bodyType: item["bodyType"] ?? "",
                      score: item["score"] ?? 0,
                      timeAgo: item["timeAgo"] ?? "",
                      image: item["image"] ?? "assets/images/p1.jpeg",
                      onTap: () {},
                    );
                  },
                ),
    );
  }
}