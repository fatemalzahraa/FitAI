import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';

class PrivacyScreen extends StatelessWidget {
  const PrivacyScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: const Text('Gizlilik'),
        backgroundColor: Colors.white,
        elevation: 0,
        foregroundColor: AppColors.textPrimary,
      ),
      body: const Padding(
        padding: EdgeInsets.all(20),
        child: Text(
          'Kullanıcı verileri güvenli şekilde saklanmaktadır.',
          style: TextStyle(fontSize: 16),
        ),
      ),
    );
  }
}