import 'package:fitai_mobile/core/theme/app_theme.dart';
import 'package:fitai_mobile/features/analysis/screens/analysis_screen.dart';
import 'package:fitai_mobile/features/favorites/screens/favorites_screen.dart';
import 'package:fitai_mobile/features/home/screens/home_screen.dart';
import 'package:fitai_mobile/features/profile/screens/profile_screen.dart';
import 'package:flutter/material.dart';


class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int _index = 0;

  Widget _buildPage() {
  switch (_index) {
    case 0:
      return const HomeScreen();
    case 1:
      return const AnalysisScreen();
    case 2:
      return const FavoritesScreen();
    case 3:
      return const ProfileScreen();
    default:
      return const HomeScreen();
  }
}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: _buildPage(),
      bottomNavigationBar: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          boxShadow: [
            BoxShadow(
              color: AppColors.primary.withOpacity(0.08),
              blurRadius: 20,
              offset: const Offset(0, -4),
            ),
          ],
        ),
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                _NavItem(icon: Icons.home_rounded, label: 'Home', active: _index == 0, onTap: () => setState(() => _index = 0)),
                _NavItem(icon: Icons.analytics_rounded, label: 'Analiz', active: _index == 1, onTap: () => setState(() => _index = 1)),
                _NavItem(icon: Icons.favorite_rounded, label: 'Favoriler', active: _index == 2, onTap: () => setState(() => _index = 2)),
                _NavItem(icon: Icons.person_rounded, label: 'Profil', active: _index == 3, onTap: () => setState(() => _index = 3)),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _NavItem extends StatelessWidget {
  final IconData icon;
  final String label;
  final bool active;
  final VoidCallback onTap;

  const _NavItem({
    required this.icon,
    required this.label,
    required this.active,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      behavior: HitTestBehavior.opaque,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          color: active ? AppColors.primary.withOpacity(0.1) : Colors.transparent,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              color: active ? AppColors.primary : AppColors.textHint,
              size: 24,
            ),
            const SizedBox(height: 2),
            Text(
              label,
              style: TextStyle(
                fontSize: 11,
                fontWeight: active ? FontWeight.w600 : FontWeight.w400,
                color: active ? AppColors.primary : AppColors.textHint,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
