import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

// ── Gradient Button ──────────────────────────────────────────────
class GradientButton extends StatelessWidget {

  final String text;

  final Future<void> Function()? onTap;

  final double? width;

  final double height;

  final Widget? leading;

  const GradientButton({

    super.key,

    required this.text,

    required this.onTap,

    this.width,

    this.height = 52,

    this.leading,
  });


  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: width ?? double.infinity,
        height: height,
        decoration: BoxDecoration(
          gradient: AppColors.primaryGradient,
          borderRadius: BorderRadius.circular(14),
          boxShadow: [
            BoxShadow(
              color: AppColors.primary.withOpacity(0.35),
              blurRadius: 16,
              offset: const Offset(0, 6),
            ),
          ],
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (leading != null) ...[leading!, const SizedBox(width: 8)],
            Text(
              text,
              style: const TextStyle(
                color: Colors.white,
                fontSize: 16,
                fontWeight: FontWeight.w600,
                letterSpacing: -0.2,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ── Logo Widget ──────────────────────────────────────────────────
class FitAILogo extends StatelessWidget {
  final double size;
  final Color? textColor;

  const FitAILogo({super.key, this.size = 24, this.textColor});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: size * 1.4,
          height: size * 1.4,
          decoration: BoxDecoration(
            gradient: AppColors.primaryGradient,
            borderRadius: BorderRadius.circular(size * 0.35),
          ),
          child: Icon(Icons.psychology_rounded, color: Colors.white, size: size * 0.85),
        ),
        const SizedBox(width: 8),
        Text(
          'FitAI',
          style: TextStyle(
            fontSize: size,
            fontWeight: FontWeight.w800,
            color: textColor ?? AppColors.textPrimary,
            letterSpacing: -0.5,
          ),
        ),
      ],
    );
  }
}

// ── Score Circle ────────────────────────────────────────────────
class ScoreCircle extends StatelessWidget {
  final int score;
  final double size;

  const ScoreCircle({super.key, required this.score, this.size = 100});

  Color get _color {
    if (score >= 80) return AppColors.scoreGreen;
    if (score >= 60) return AppColors.warning;
    return AppColors.error;
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: size,
      height: size,
      child: Stack(
        alignment: Alignment.center,
        children: [
          SizedBox(
            width: size,
            height: size,
            child: CircularProgressIndicator(
              value: score / 100,
              strokeWidth: size * 0.08,
              backgroundColor: AppColors.divider,
              valueColor: AlwaysStoppedAnimation(_color),
              strokeCap: StrokeCap.round,
            ),
          ),
          Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                '$score%',
                style: TextStyle(
                  fontSize: size * 0.28,
                  fontWeight: FontWeight.w800,
                  color: _color,
                  letterSpacing: -1,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

// ── Platform Badge ──────────────────────────────────────────────
class PlatformBadge extends StatelessWidget {
  final String platform;

  const PlatformBadge({super.key, required this.platform});

  @override
  Widget build(BuildContext context) {
    final isTrendyol = platform.toLowerCase().contains('trendyol');
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: (isTrendyol ? AppColors.trendyolOrange : AppColors.hepsiburadaBlue).withOpacity(0.12),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Text(
        platform,
        style: TextStyle(
          fontSize: 11,
          fontWeight: FontWeight.w600,
          color: isTrendyol ? AppColors.trendyolOrange : AppColors.hepsiburadaBlue,
        ),
      ),
    );
  }
}

// ── Body Type Icon ──────────────────────────────────────────────
class BodyTypeCard extends StatelessWidget {
  final String name;
  final bool selected;
  final VoidCallback onTap;

  const BodyTypeCard({
    super.key,
    required this.name,
    required this.selected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        decoration: BoxDecoration(
          color: selected ? AppColors.primary.withOpacity(0.12) : Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(
            color: selected ? AppColors.primary : AppColors.divider,
            width: selected ? 2 : 1,
          ),
        ),
        padding: const EdgeInsets.symmetric(vertical: 16),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _BodySilhouette(type: name, selected: selected),
            const SizedBox(height: 8),
            Text(
              name,
              style: TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w600,
                color: selected ? AppColors.primary : AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _BodySilhouette extends StatelessWidget {
  final String type;
  final bool selected;

  const _BodySilhouette({required this.type, required this.selected});

  @override
  Widget build(BuildContext context) {
    final color = selected ? AppColors.primary : AppColors.accent;
    return CustomPaint(
      size: const Size(44, 60),
      painter: _SilhouettePainter(type: type, color: color),
    );
  }
}

class _SilhouettePainter extends CustomPainter {
  final String type;
  final Color color;

  _SilhouettePainter({required this.type, required this.color});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = color;
    final w = size.width;
    final h = size.height;

    // Draw simple body silhouettes based on type
    switch (type.toLowerCase()) {
      case 'armut':
        // Pear - narrow top, wide bottom
        final path = Path()
          ..moveTo(w * 0.35, h * 0.05)
          ..quadraticBezierTo(w * 0.65, h * 0.05, w * 0.65, h * 0.05)
          ..quadraticBezierTo(w * 0.7, h * 0.3, w * 0.75, h * 0.55)
          ..quadraticBezierTo(w * 0.8, h * 0.9, w * 0.5, h * 0.95)
          ..quadraticBezierTo(w * 0.2, h * 0.9, w * 0.25, h * 0.55)
          ..quadraticBezierTo(w * 0.3, h * 0.3, w * 0.35, h * 0.05)
          ..close();
        canvas.drawPath(path, paint);
        break;
      case 'kum saati':
        // Hourglass
        final path = Path()
          ..moveTo(w * 0.2, h * 0.02)
          ..lineTo(w * 0.8, h * 0.02)
          ..quadraticBezierTo(w * 0.75, h * 0.35, w * 0.5, h * 0.5)
          ..quadraticBezierTo(w * 0.75, h * 0.65, w * 0.8, h * 0.98)
          ..lineTo(w * 0.2, h * 0.98)
          ..quadraticBezierTo(w * 0.25, h * 0.65, w * 0.5, h * 0.5)
          ..quadraticBezierTo(w * 0.25, h * 0.35, w * 0.2, h * 0.02)
          ..close();
        canvas.drawPath(path, paint);
        break;
      default:
        // Default rectangle body
        final rrect = RRect.fromRectAndRadius(
          Rect.fromLTWH(w * 0.2, h * 0.02, w * 0.6, h * 0.96),
          const Radius.circular(12),
        );
        canvas.drawRRect(rrect, paint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

// ── Analysis Card ────────────────────────────────────────────────
class AnalysisCard extends StatelessWidget {
  final String productName;
  final String platform;
  final String bodyType;
  final int score;
  final String? imageUrl;
  final String timeAgo;
  final String image;
  final VoidCallback onTap;

  const AnalysisCard({
    super.key,
    required this.productName,
    required this.platform,
    required this.bodyType,
    required this.score,
    this.imageUrl,
    required this.timeAgo,
    required this.image,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.only(bottom: 12),
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppColors.divider),
        ),
        child: Row(
          children: [
            ClipRRect(
  borderRadius: BorderRadius.circular(10),
  child: Container(
    width: 68,
    height: 68,
    color: AppColors.background,
    child: Image.asset(
      image,
      fit: BoxFit.cover,
    ),
  ),
),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      PlatformBadge(platform: platform),
                      const SizedBox(width: 6),
                      Text(timeAgo, style: const TextStyle(fontSize: 11, color: AppColors.textHint)),
                    ],
                  ),
                  const SizedBox(height: 5),
                  Text(
                    productName,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.w600,
                      color: AppColors.textPrimary,
                      height: 1.3,
                    ),
                  ),
                  const SizedBox(height: 6),
                  Row(
                    children: [
                      _ScoreBadge(score: score),
                      const SizedBox(width: 8),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 7, vertical: 3),
                        decoration: BoxDecoration(
                          color: AppColors.background,
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Text(
                          bodyType,
                          style: const TextStyle(
                            fontSize: 11,
                            color: AppColors.textSecondary,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(width: 8),
            ElevatedButton(
              onPressed: onTap,
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.primary,
                foregroundColor: Colors.white,
                elevation: 0,
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                minimumSize: Size.zero,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                textStyle: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
              ),
              child: const Text('Detayları Gör'),
            ),
          ],
        ),
      ),
    );
  }
}

class _ScoreBadge extends StatelessWidget {
  final int score;

  const _ScoreBadge({required this.score});

  Color get _color {
    if (score >= 80) return AppColors.scoreGreen;
    if (score >= 60) return AppColors.warning;
    return AppColors.error;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: _color.withOpacity(0.12),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            '$score%',
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w700,
              color: _color,
            ),
          ),
          const SizedBox(width: 3),
          Icon(Icons.arrow_upward_rounded, size: 11, color: _color),
          Text(
            score >= 80 ? 'Yüksek' : score >= 60 ? 'Orta' : 'Düşük',
            style: TextStyle(fontSize: 11, color: _color, fontWeight: FontWeight.w500),
          ),
        ],
      ),
    );
  }
}
