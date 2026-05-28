import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';

class AnalysisScreen extends StatefulWidget {
  const AnalysisScreen({super.key});

  @override
  State<AnalysisScreen> createState() => _AnalysisScreenState();
}

class _AnalysisScreenState extends State<AnalysisScreen> {
  final _linkCtrl = TextEditingController();
  bool _analyzing = false;

  Future<void> _analyze() async {
    if (_linkCtrl.text.isEmpty) return;
    setState(() => _analyzing = true);
    await Future.delayed(const Duration(seconds: 2));
    if (mounted) {
      setState(() => _analyzing = false);
      Navigator.pushNamed(context, AppRoutes.productDetail);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: const BoxDecoration(gradient: AppColors.bgGradient),
      child: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const SizedBox(height: 20),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text(
                    'Ürün Analizi',
                    style: TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.w800,
                      color: AppColors.textPrimary,
                      letterSpacing: -0.5,
                    ),
                  ),
                  Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: AppColors.divider),
                    ),
                    child: const Icon(Icons.history_rounded,
                        color: AppColors.primary, size: 22),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              const Text(
                'Ürün linkini girerek AI destekli\nvücut uyum analizini başlat.',
                style: TextStyle(
                  fontSize: 14,
                  color: AppColors.textSecondary,
                  height: 1.4,
                ),
              ),
              const SizedBox(height: 28),

              // Platform icons
              Row(
                children: [
                  _PlatformChip(label: 'Trendyol', color: AppColors.trendyolOrange),
                  const SizedBox(width: 8),
                  _PlatformChip(label: 'Hepsiburada', color: AppColors.hepsiburadaBlue),
                ],
              ),
              const SizedBox(height: 20),

              // Input
              Container(
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: AppColors.primary.withOpacity(0.3)),
                  boxShadow: [
                    BoxShadow(
                      color: AppColors.primary.withOpacity(0.06),
                      blurRadius: 12,
                      offset: const Offset(0, 4),
                    ),
                  ],
                ),
                child: TextField(
                  controller: _linkCtrl,
                  maxLines: 3,
                  decoration: const InputDecoration(
                    hintText: 'https://www.trendyol.com/...',
                    hintStyle: TextStyle(color: AppColors.textHint, fontSize: 14),
                    border: InputBorder.none,
                    enabledBorder: InputBorder.none,
                    focusedBorder: InputBorder.none,
                    contentPadding: EdgeInsets.all(18),
                  ),
                ),
              ),
              const SizedBox(height: 16),

              _analyzing
                  ? _LoadingAnalysis()
                  : GradientButton(
                      text: 'Analiz Et',
                      onTap: _analyze,
                      leading: const Icon(Icons.auto_awesome_rounded,
                          color: Colors.white, size: 18),
                    ),
              const SizedBox(height: 32),

              // Info cards
              const Text(
                'Nasıl Çalışır?',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w700,
                  color: AppColors.textPrimary,
                ),
              ),
              const SizedBox(height: 12),
              _HowItWorksCard(
                step: '1',
                title: 'Ürün Linkini Gir',
                desc: 'Trendyol veya Hepsiburada ürün sayfasının linkini kopyala yapıştır.',
                icon: Icons.link_rounded,
              ),
              const SizedBox(height: 8),
              _HowItWorksCard(
                step: '2',
                title: 'AI Analiz',
                desc: 'Yapay zeka ürün özelliklerini ve yorumları tarar.',
                icon: Icons.psychology_rounded,
              ),
              const SizedBox(height: 8),
              _HowItWorksCard(
                step: '3',
                title: 'Uyum Skoru',
                desc: 'Vücut tipine özel uyum skoru ve öneriler al.',
                icon: Icons.analytics_rounded,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _PlatformChip extends StatelessWidget {
  final String label;
  final Color color;

  const _PlatformChip({required this.label, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Text(
        label,
        style: TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.w600,
          color: color,
        ),
      ),
    );
  }
}

class _LoadingAnalysis extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      height: 52,
      decoration: BoxDecoration(
        gradient: AppColors.primaryGradient,
        borderRadius: BorderRadius.circular(14),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const SizedBox(
            width: 20,
            height: 20,
            child: CircularProgressIndicator(
              color: Colors.white,
              strokeWidth: 2.5,
            ),
          ),
          const SizedBox(width: 12),
          Text(
            'AI Analiz Ediliyor...',
            style: const TextStyle(
              color: Colors.white,
              fontSize: 16,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}

class _HowItWorksCard extends StatelessWidget {
  final String step;
  final String title;
  final String desc;
  final IconData icon;

  const _HowItWorksCard(
      {required this.step,
      required this.title,
      required this.desc,
      required this.icon});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.divider),
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: AppColors.primary.withOpacity(0.1),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Icon(icon, color: AppColors.primary, size: 22),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: const TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  desc,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.textSecondary,
                    height: 1.3,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
