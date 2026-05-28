import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';

class ReviewAnalysisScreen extends StatefulWidget {
  const ReviewAnalysisScreen({super.key});

  @override
  State<ReviewAnalysisScreen> createState() => _ReviewAnalysisScreenState();
}

class _ReviewAnalysisScreenState extends State<ReviewAnalysisScreen> {
  String _filter = 'Tümü';

  final _filters = ['Tümü', 'Pozitif', 'Negatif', 'Beden', 'Kumaş'];

  final _reviews = [
    {
      'text': 'Gerçekten çok güzel bir gömlek, kumaşı kaliteli ve dikişler sağlam.',
      'sentiment': 'positive',
      'issue': null,
      'time': '2 gün önce',
    },
    {
      'text': 'Beden biraz büyük geldi, bir küçük almalıydım. Ama kalitesi iyi.',
      'sentiment': 'neutral',
      'issue': 'Beden',
      'time': '5 gün önce',
    },
    {
      'text': 'Kumaş biraz sentetik hissettiriyor, doğal kumaş bekledim.',
      'sentiment': 'negative',
      'issue': 'Kumaş',
      'time': '1 hafta önce',
    },
    {
      'text': 'Renk fotoğraftakiyle birebir aynı. Çok memnun kaldım.',
      'sentiment': 'positive',
      'issue': null,
      'time': '2 hafta önce',
    },
    {
      'text': 'Omuz kısmı biraz dar geldi, omuzları geniş olanlar dikkat.',
      'sentiment': 'negative',
      'issue': 'Beden',
      'time': '3 hafta önce',
    },
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Yorum Analizi'),
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
            // Sentiment summary
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(18),
                border: Border.all(color: AppColors.divider),
              ),
              child: Column(
                children: [
                  const Text(
                    'Duygu Analizi Özeti',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w700,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 16),
                  Row(
                    children: [
                      _SentimentBar(label: 'Pozitif', percent: 68, color: AppColors.scoreGreen),
                      const SizedBox(width: 12),
                      _SentimentBar(label: 'Nötr', percent: 18, color: AppColors.warning),
                      const SizedBox(width: 12),
                      _SentimentBar(label: 'Negatif', percent: 14, color: AppColors.error),
                    ],
                  ),
                  const SizedBox(height: 16),
                  // Stacked bar
                  ClipRRect(
                    borderRadius: BorderRadius.circular(6),
                    child: Row(
                      children: [
                        Expanded(
                          flex: 68,
                          child: Container(height: 10, color: AppColors.scoreGreen),
                        ),
                        Expanded(
                          flex: 18,
                          child: Container(height: 10, color: AppColors.warning),
                        ),
                        Expanded(
                          flex: 14,
                          child: Container(height: 10, color: AppColors.error),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),

            // Issue breakdown
            const Text(
              'Sık Bahsedilen Konular',
              style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.w700,
                color: AppColors.textPrimary,
              ),
            ),
            const SizedBox(height: 10),
            Row(
              children: [
                _IssueBadge(label: 'Beden', count: 12, color: AppColors.warning),
                const SizedBox(width: 8),
                _IssueBadge(label: 'Kumaş', count: 8, color: AppColors.primary),
                const SizedBox(width: 8),
                _IssueBadge(label: 'Renk', count: 5, color: AppColors.scoreGreen),
                const SizedBox(width: 8),
                _IssueBadge(label: 'Kargo', count: 3, color: AppColors.textHint),
              ],
            ),
            const SizedBox(height: 20),

            // Filter chips
            const Text(
              'Yorumlar',
              style: TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.w700,
                color: AppColors.textPrimary,
              ),
            ),
            const SizedBox(height: 10),
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: _filters.map((f) {
                  final active = _filter == f;
                  return GestureDetector(
                    onTap: () => setState(() => _filter = f),
                    child: AnimatedContainer(
                      duration: const Duration(milliseconds: 200),
                      margin: const EdgeInsets.only(right: 8),
                      padding: const EdgeInsets.symmetric(
                          horizontal: 14, vertical: 7),
                      decoration: BoxDecoration(
                        color: active ? AppColors.primary : Colors.white,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                            color: active
                                ? AppColors.primary
                                : AppColors.divider),
                      ),
                      child: Text(
                        f,
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                          color: active ? Colors.white : AppColors.textSecondary,
                        ),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
            const SizedBox(height: 12),

            ..._reviews.map((r) => _ReviewCard(
                  text: r['text'] as String,
                  sentiment: r['sentiment'] as String,
                  issue: r['issue'] as String?,
                  time: r['time'] as String,
                )),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }
}

class _SentimentBar extends StatelessWidget {
  final String label;
  final int percent;
  final Color color;

  const _SentimentBar(
      {required this.label, required this.percent, required this.color});

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Column(
        children: [
          Text(
            '$percent%',
            style: TextStyle(
              fontSize: 20,
              fontWeight: FontWeight.w800,
              color: color,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            label,
            style: const TextStyle(
                fontSize: 12, color: AppColors.textSecondary),
          ),
        ],
      ),
    );
  }
}

class _IssueBadge extends StatelessWidget {
  final String label;
  final int count;
  final Color color;

  const _IssueBadge(
      {required this.label, required this.count, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: color.withOpacity(0.25)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            label,
            style: TextStyle(
                fontSize: 12, fontWeight: FontWeight.w600, color: color),
          ),
          const SizedBox(width: 4),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 1),
            decoration: BoxDecoration(
              color: color,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Text(
              '$count',
              style: const TextStyle(
                  fontSize: 10, fontWeight: FontWeight.w700, color: Colors.white),
            ),
          ),
        ],
      ),
    );
  }
}

class _ReviewCard extends StatelessWidget {
  final String text;
  final String sentiment;
  final String? issue;
  final String time;

  const _ReviewCard(
      {required this.text,
      required this.sentiment,
      this.issue,
      required this.time});

  Color get _sentimentColor {
    switch (sentiment) {
      case 'positive':
        return AppColors.scoreGreen;
      case 'negative':
        return AppColors.error;
      default:
        return AppColors.warning;
    }
  }

  IconData get _sentimentIcon {
    switch (sentiment) {
      case 'positive':
        return Icons.sentiment_very_satisfied_rounded;
      case 'negative':
        return Icons.sentiment_very_dissatisfied_rounded;
      default:
        return Icons.sentiment_neutral_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.divider),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(_sentimentIcon, color: _sentimentColor, size: 18),
              const SizedBox(width: 6),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                decoration: BoxDecoration(
                  color: _sentimentColor.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(5),
                ),
                child: Text(
                  sentiment == 'positive'
                      ? 'Pozitif'
                      : sentiment == 'negative'
                          ? 'Negatif'
                          : 'Nötr',
                  style: TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.w600,
                    color: _sentimentColor,
                  ),
                ),
              ),
              if (issue != null) ...[
                const SizedBox(width: 6),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withOpacity(0.08),
                    borderRadius: BorderRadius.circular(5),
                  ),
                  child: Text(
                    issue!,
                    style: const TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                      color: AppColors.primary,
                    ),
                  ),
                ),
              ],
              const Spacer(),
              Text(
                time,
                style: const TextStyle(
                    fontSize: 11, color: AppColors.textHint),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            text,
            style: const TextStyle(
              fontSize: 13,
              color: AppColors.textPrimary,
              height: 1.4,
            ),
          ),
        ],
      ),
    );
  }
}
