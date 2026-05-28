import 'package:fitai_mobile/features/analysis/screens/ai_recommendation_screen.dart';
import 'package:fitai_mobile/features/analysis/screens/all_analysis_screen.dart';
import 'package:fitai_mobile/features/notifications/screens/notification_screen.dart';
import 'package:flutter/material.dart';
import 'core/theme/app_theme.dart';
import 'core/constants/app_routes.dart';
import 'features/auth/screens/splash_screen.dart';
import 'features/auth/screens/login_screen.dart';
import 'features/auth/screens/register_screen.dart';
import 'features/onboarding/screens/body_type_screen.dart';
import 'features/home/screens/main_screen.dart';
import 'features/analysis/screens/product_detail_screen.dart';
import 'features/analysis/screens/fit_score_screen.dart';
import 'features/analysis/screens/review_analysis_screen.dart';
import 'features/analysis/screens/analysis_screen.dart';
import 'features/favorites/screens/favorites_screen.dart';
import 'features/profile/screens/profile_screen.dart';
import 'features/profile/screens/settings_screen.dart';
import 'features/edit_profile/screens/edit_profile_screen.dart';
import 'features/analysis_history/screens/analysis_history_screen.dart';
import 'features/privacy/screens/privacy_screen.dart';
import 'features/help/screens/help_screen.dart';
void main() {
  runApp(const FitAIApp());
}

class FitAIApp extends StatelessWidget {
  const FitAIApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'FitAI',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.theme,
      initialRoute: AppRoutes.splash,
      routes: {
        AppRoutes.splash: (_) => const SplashScreen(),
        AppRoutes.login: (_) => const LoginScreen(),
        AppRoutes.register: (_) => const RegisterScreen(),
        AppRoutes.bodyType: (_) => const BodyTypeScreen(),
        AppRoutes.main: (_) => const MainScreen(),
        AppRoutes.productDetail: (_) => const ProductDetailScreen(),
        AppRoutes.fitScore: (_) => const FitScoreScreen(),
        AppRoutes.reviewAnalysis: (_) => const ReviewAnalysisScreen(),
        AppRoutes.notifications: (_) => const NotificationScreen(),
AppRoutes.aiRecommendations: (_) => const AiRecommendationScreen(),
AppRoutes.allAnalysis: (_) => const AllAnalysisScreen(),
AppRoutes.analysis: (_) => const AnalysisScreen(),
AppRoutes.favorites: (_) => const FavoritesScreen(),

AppRoutes.profile: (_) => const ProfileScreen(),

AppRoutes.settings: (_) => const SettingsScreen(),
AppRoutes.settings: (context) => const SettingsScreen(),
  AppRoutes.editProfile: (context) => const EditProfileScreen(),
  AppRoutes.notifications: (context) => const NotificationScreen(),
  AppRoutes.analysisHistory: (context) => const AnalysisHistoryScreen(),
  AppRoutes.privacy: (context) => const PrivacyScreen(),
  AppRoutes.help: (context) => const HelpScreen(),
      },
    );
  }
}
