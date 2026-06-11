import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';

class AnalysisHistoryScreen extends StatelessWidget {
  const AnalysisHistoryScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: const Text('Analiz Geçmişi'),
        backgroundColor: Colors.white,
        elevation: 0,
        foregroundColor: AppColors.textPrimary,
      ),
      body: ListView(
        children: const [
          ListTile(
            leading: Icon(Icons.analytics_outlined),
            title: Text('Denim Gömlek Analizi'),
            subtitle: Text('Skor: %73'),
          ),
          ListTile(
            leading: Icon(Icons.analytics_outlined),
            title: Text('Kazak Analizi'),
            subtitle: Text('Skor: %94'),
          ),
        ],
      ),
    );
  }
}