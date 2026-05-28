import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';
import '../../../core/services/api_service.dart';
import 'package:shared_preferences/shared_preferences.dart';
class BodyTypeScreen extends StatefulWidget {
  const BodyTypeScreen({super.key});

  @override
  State<BodyTypeScreen> createState() => _BodyTypeScreenState();
}

class _BodyTypeScreenState extends State<BodyTypeScreen> {
  String? _selected;
 final ApiService _api = ApiService();
  final List<Map<String, dynamic>> _bodyTypes = [
    {'name': 'Armut', 'icon': Icons.expand_more_rounded},
    {'name': 'Kum Saati', 'icon': Icons.hourglass_empty_rounded},
    {'name': 'Dikdörtgen', 'icon': Icons.crop_din_rounded},
    {'name': 'Çilek', 'icon': Icons.arrow_drop_up_rounded},
    {'name': 'Oval', 'icon': Icons.circle_outlined},
    {'name': 'Ters Üçgen', 'icon': Icons.change_history_rounded},
    {'name': 'Üçgen', 'icon': Icons.details_rounded},
    {'name': 'Usun', 'icon': Icons.straighten_rounded},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(gradient: AppColors.bgGradient),
        child: SafeArea(
          child: Column(
            children: [
              // Header
              Padding(
                padding: const EdgeInsets.fromLTRB(24, 20, 24, 0),
                child: Row(
                  children: [
                    GestureDetector(
                      onTap: () => Navigator.pop(context),
                      child: Container(
                        width: 40,
                        height: 40,
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(color: AppColors.divider),
                        ),
                        child: const Icon(Icons.arrow_back_ios_rounded,
                            color: AppColors.textPrimary, size: 18),
                      ),
                    ),
                    const SizedBox(width: 16),
                    const Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Vücut Tipi Seçimi',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.w700,
                            color: AppColors.textPrimary,
                            letterSpacing: -0.3,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 28),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Vücut tipinizi seçin',
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w800,
                        color: AppColors.textPrimary,
                        letterSpacing: -0.5,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'AI analizimiz vücut tipinize göre\nkişiselleştirilmiş öneriler sunar.',
                      style: TextStyle(
                        fontSize: 14,
                        color: AppColors.textSecondary,
                        height: 1.4,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  child: GridView.builder(
                    physics: const BouncingScrollPhysics(),
                    gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                      crossAxisCount: 3,
                      mainAxisSpacing: 12,
                      crossAxisSpacing: 12,
                      childAspectRatio: 0.85,
                    ),
                    itemCount: _bodyTypes.length,
                    itemBuilder: (context, i) {
                      final type = _bodyTypes[i];
                      final name = type['name'] as String;
                      final selected = _selected == name;
                      return GestureDetector(
                        onTap: () => setState(() => _selected = name),
                        child: AnimatedContainer(
                          duration: const Duration(milliseconds: 200),
                          decoration: BoxDecoration(
                            color: selected
                                ? AppColors.primary.withOpacity(0.1)
                                : Colors.white,
                            borderRadius: BorderRadius.circular(16),
                            border: Border.all(
                              color: selected
                                  ? AppColors.primary
                                  : AppColors.divider,
                              width: selected ? 2 : 1,
                            ),
                          ),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Container(
                                width: 52,
                                height: 60,
                                decoration: BoxDecoration(
                                  color: selected
                                      ? AppColors.primary.withOpacity(0.15)
                                      : AppColors.background,
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: Icon(
                                  type['icon'] as IconData,
                                  color: selected
                                      ? AppColors.primary
                                      : AppColors.accent,
                                  size: 32,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Text(
                                name,
                                textAlign: TextAlign.center,
                                style: TextStyle(
                                  fontSize: 12,
                                  fontWeight: FontWeight.w600,
                                  color: selected
                                      ? AppColors.primary
                                      : AppColors.textSecondary,
                                ),
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(24, 12, 24, 24),
                child: GradientButton(
                  text: _selected == null ? 'Devam Et' : '$_selected → Devam Et',
                onTap: () async {
  if (_selected == null) return;

  final prefs = await SharedPreferences.getInstance();
  final token = prefs.getString("token");

  if (token == null) return;

  try {
await _api.updateBodyType(_selected!); 
  print("BODYTYPE SAVED");

    if (!mounted) return;

    Navigator.pushReplacementNamed(
      context,
      AppRoutes.main,
    );
  } catch (e) {
    print("BODYTYPE ERROR: $e");

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text("Kayıt başarısız (401)")),
    );
  }
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
