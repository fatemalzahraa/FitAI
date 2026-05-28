import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';

class FitScoreScreen extends StatefulWidget {
  const FitScoreScreen({super.key});

  @override
  State<FitScoreScreen> createState() => _FitScoreScreenState();
}

class _FitScoreScreenState extends State<FitScoreScreen>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scoreAnim;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 1200));
    _scoreAnim =
        Tween<double>(begin: 0, end: 0.85).animate(CurvedAnimation(
      parent: _controller,
      curve: Curves.easeOutCubic,
    ));
    _controller.forward();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Fit Score Analizi'),
        leading: GestureDetector(
          onTap: () => Navigator.pop(context),
          child: Container(
            margin: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(10),
              border: Border.all(color: AppColors.divider),
            ),
            child: const Icon(Icons.arrow_back_ios_rounded,
                color: AppColors.textPrimary, size: 18),
          ),
        ),
      ),
      body: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Score Hero Card
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [Color(0xFF5B3FD4), Color(0xFF9B7EFD)],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(24),
              ),
              child: Column(
                children: [
                  AnimatedBuilder(
                    animation: _scoreAnim,
                    builder: (context, _) {
                      final score = (_scoreAnim.value * 100).round();
                      return SizedBox(
                        width: 140,
                        height: 140,
                        child: Stack(
                          alignment: Alignment.center,
                          children: [
                            SizedBox(
                              width: 140,
                              height: 140,
                              child: CircularProgressIndicator(
                                value: _scoreAnim.value,
                                strokeWidth: 12,
                                backgroundColor: Colors.white.withOpacity(0.2),
                                valueColor: const AlwaysStoppedAnimation(Colors.white),
                                strokeCap: StrokeCap.round,
                              ),
                            ),
                            Column(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Text(
                                  '$score%',
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 36,
                                    fontWeight: FontWeight.w800,
                                    letterSpacing: -1,
                                  ),
                                ),
                                Text(
                                  'Vücut uyumu',
                                  style: TextStyle(
                                    color: Colors.white.withOpacity(0.8),
                                    fontSize: 12,
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Vücut uyum iyi görünüyor ✓',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Bu ürün vücut tipinle yüksek uyum gösteriyor.',
                    style: TextStyle(
                      color: Colors.white.withOpacity(0.75),
                      fontSize: 13,
                    ),
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // Risk & Recommendation cards
            Row(
              children: [
                Expanded(
                  child: _InfoCard(
                    title: 'İade Riski',
                    value: 'Yüksek',
                    icon: Icons.warning_amber_rounded,
                    color: AppColors.warning,
                    desc: 'Beden uyumsuzluğu riski mevcut',
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _InfoCard(
                    title: 'Beden Önerisi',
                    value: 'Büyük Al',
                    icon: Icons.straighten_rounded,
                    color: AppColors.primary,
                    desc: 'Normal bedeninden 1 büyük seç',
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Detailed breakdown
            _SectionTitle('Detaylı Analiz'),
            const SizedBox(height: 12),
            _ScoreRow(label: 'Omuz Genişliği', score: 88),
            const SizedBox(height: 10),
            _ScoreRow(label: 'Göğüs Çevresi', score: 92),
            const SizedBox(height: 10),
            _ScoreRow(label: 'Bel Çevresi', score: 75),
            const SizedBox(height: 10),
            _ScoreRow(label: 'Kumaş Esnekliği', score: 80),
            const SizedBox(height: 20),

            _SectionTitle('AI Önerileri'),
            const SizedBox(height: 12),
            _RecommendationCard(
              icon: Icons.tips_and_updates_rounded,
              color: AppColors.primary,
              text: 'Bu ürün armut vücut tipine uygundur. Omuz genişliği iyi uyum sağlar.',
            ),
            const SizedBox(height: 8),
            _RecommendationCard(
              icon: Icons.warning_amber_rounded,
              color: AppColors.warning,
              text: 'Kumaş yapısı sert. Beden büyük alarak hareket rahatlığı sağlayabilirsin.',
            ),
            const SizedBox(height: 8),
            _RecommendationCard(
              icon: Icons.check_circle_outline_rounded,
              color: AppColors.scoreGreen,
              text: 'Renk ve model vücut hatlarını ön plana çıkarıyor.',
            ),
            const SizedBox(height: 24),

            Row(
              children: [
                Expanded(
                  child: GradientButton(
                    text: 'Satın Al',
                    onTap: () async {},
                    height: 48,
                  ),
                ),
                const SizedBox(width: 10),
                Container(
                  width: 48,
                  height: 48,
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(14),
                    border: Border.all(color: AppColors.divider),
                  ),
                  child: IconButton(
                    icon: const Icon(Icons.favorite_border_rounded,
                        color: AppColors.error, size: 20),
                    onPressed: () {},
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              height: 48,
              child: OutlinedButton.icon(
                onPressed: () =>
                    Navigator.pushNamed(context, AppRoutes.reviewAnalysis),
                icon: const Icon(Icons.comment_rounded, size: 18),
                label: const Text('Yorum Analizini Gör'),
                style: OutlinedButton.styleFrom(
                  side: const BorderSide(color: AppColors.primary),
                  foregroundColor: AppColors.primary,
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(14)),
                  textStyle: const TextStyle(
                    fontWeight: FontWeight.w600,
                    fontSize: 14,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }
}

class _SectionTitle extends StatelessWidget {
  final String text;
  const _SectionTitle(this.text);

  @override
  Widget build(BuildContext context) {
    return Text(
      text,
      style: const TextStyle(
        fontSize: 16,
        fontWeight: FontWeight.w700,
        color: AppColors.textPrimary,
        letterSpacing: -0.2,
      ),
    );
  }
}

class _InfoCard extends StatelessWidget {
  final String title;
  final String value;
  final IconData icon;
  final Color color;
  final String desc;

  const _InfoCard(
      {required this.title,
      required this.value,
      required this.icon,
      required this.color,
      required this.desc});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withOpacity(0.08),
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: color.withOpacity(0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: color, size: 22),
          const SizedBox(height: 8),
          Text(
            value,
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w800,
              color: color,
            ),
          ),
          Text(
            title,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            desc,
            style: const TextStyle(fontSize: 11, color: AppColors.textHint),
          ),
        ],
      ),
    );
  }
}

class _ScoreRow extends StatelessWidget {
  final String label;
  final int score;

  const _ScoreRow({required this.label, required this.score});

  @override
  Widget build(BuildContext context) {
    final color = score >= 80
        ? AppColors.scoreGreen
        : score >= 60
            ? AppColors.warning
            : AppColors.error;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              label,
              style: const TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w500,
                  color: AppColors.textPrimary),
            ),
            Text(
              '$score%',
              style: TextStyle(
                  fontSize: 13, fontWeight: FontWeight.w700, color: color),
            ),
          ],
        ),
        const SizedBox(height: 6),
        ClipRRect(
          borderRadius: BorderRadius.circular(4),
          child: LinearProgressIndicator(
            value: score / 100,
            minHeight: 6,
            backgroundColor: AppColors.divider,
            valueColor: AlwaysStoppedAnimation(color),
          ),
        ),
      ],
    );
  }
}

class _RecommendationCard extends StatelessWidget {
  final IconData icon;
  final Color color;
  final String text;

  const _RecommendationCard(
      {required this.icon, required this.color, required this.text});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.divider),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(6),
            decoration: BoxDecoration(
              color: color.withOpacity(0.1),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Icon(icon, color: color, size: 16),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              text,
              style: const TextStyle(
                fontSize: 13,
                color: AppColors.textPrimary,
                height: 1.4,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
