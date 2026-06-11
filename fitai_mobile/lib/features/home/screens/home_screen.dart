import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';
import '../../favorites/screens/favorites_screen.dart';
import '../../profile/screens/profile_screen.dart';
import '../../../core/services/api_service.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _linkCtrl = TextEditingController();
  int _tabIndex = 0;
String name = "";
final ApiService _api = ApiService();
  final _analyses = [
    {
      'name': 'Kudos Açık Mavi Denim Gömlek',
      'platform': 'Trendyol',
      'score': 73,
      'bodyType': 'Armut',
      'time': '12 saat önce',
      'image': 'assets/images/p1.jpeg',
    },
    {
      'name': 'Jakamon Yeşil Kaşe Kazak',
      'platform': 'Trendyol',
      'score': 94,
      'bodyType': 'Armut',
      'time': '3 saat önce',
      'image': 'assets/images/p2.jpeg',
    },
    {
      'name': 'Los Ocampo Kahverengi Sneaker',
      'platform': 'Trendyol',
      'score': 89,
      'bodyType': 'Armut',
      'time': 'Dün',
      'image': 'assets/images/p3.jpeg',
    },
  ];
  @override
void initState() {
  super.initState();
  loadUserName();
}
Future<void> loadUserName() async {
  final prefs = await SharedPreferences.getInstance();

  try {
    final res = await _api.getMyProfile();
    final data = res.data;

    await prefs.setString('userName', data["name"]);

    setState(() {
      name = data["name"] ?? "";
    });
  } catch (e) {
    setState(() {
      name = prefs.getString('userName') ?? "";
    });
  }
}
Widget _buildDrawer() {
  return Drawer(
    backgroundColor: Colors.white,
    child: SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const FitAILogo(size: 26),
            const SizedBox(height: 30),

            _drawerItem(
              icon: Icons.home_rounded,
              title: 'Ana Sayfa',
              onTap: () {
                Navigator.pop(context);
              },
            ),

            _drawerItem(
              icon: Icons.analytics_rounded,
              title: 'Analizler',
              onTap: () {
                Navigator.pushNamed(context, AppRoutes.allAnalysis);
              },
            ),

            _drawerItem(
              icon: Icons.favorite_rounded,
              title: 'Favoriler',
              onTap: () {
                Navigator.pushNamed(context, AppRoutes.favorites);
              },
            ),

            _drawerItem(
              icon: Icons.person_rounded,
              title: 'Profil',
              onTap: () {
                Navigator.pushNamed(context, AppRoutes.profile);
              },
            ),

            _drawerItem(
              icon: Icons.settings_rounded,
              title: 'Ayarlar',
              onTap: () {
                Navigator.pushNamed(context, AppRoutes.settings);
              },
            ),
          ],
        ),
      ),
    ),
  );
}

Widget _drawerItem({
  required IconData icon,
  required String title,
  required VoidCallback onTap,
}) {
  return Padding(
    padding: const EdgeInsets.only(bottom: 10),
    child: ListTile(
      onTap: onTap,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(14),
      ),
      leading: Icon(icon, color: AppColors.primary),
      title: Text(
        title,
        style: const TextStyle(
          fontWeight: FontWeight.w600,
          color: AppColors.textPrimary,
        ),
      ),
    ),
  );
}
  @override
  Widget build(BuildContext context) {
    return Scaffold(
  drawer: _buildDrawer(),
  
  backgroundColor: Colors.transparent,
  body: Container(
      decoration: const BoxDecoration(gradient: AppColors.bgGradient),
      child: SafeArea(
        child: CustomScrollView(
          physics: const BouncingScrollPhysics(),
          slivers: [
            SliverToBoxAdapter(child: _buildHeader()),
            SliverToBoxAdapter(child: _buildSearchBar()),
            SliverToBoxAdapter(child: _buildAiSection()),
            SliverToBoxAdapter(child: _buildTabBar()),
            SliverPadding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              sliver: SliverList(
                delegate: SliverChildBuilderDelegate(
                  (context, i) {
                    final item = _analyses[i];
                    return AnalysisCard(
                      productName: item['name'] as String,
                      platform: item['platform'] as String,
                      score: item['score'] as int,
                      bodyType: item['bodyType'] as String,
                      timeAgo: item['time'] as String,
                      image: item['image'] as String,
                      onTap: () => Navigator.pushNamed(
                          context, AppRoutes.productDetail),
                    );
                  },
                  childCount: _analyses.length,
                ),
              ),
            ),
            const SliverToBoxAdapter(child: SizedBox(height: 20)),
          ],
        ),
      ),
  ),
    );
  }

  Widget _buildHeader() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
            Builder(
  builder: (context) => GestureDetector(
    onTap: () => Scaffold.of(context).openDrawer(),
    child: const Icon(
      Icons.menu_rounded,
      color: AppColors.textPrimary,
      size: 24,
    ),
  ),
),
              const SizedBox(width: 12),
              const FitAILogo(size: 20),
            ],
          ),
        
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          RichText(
            text:  TextSpan(
              text: 'Merhaba $name! ',
              style: TextStyle(
                fontSize: 22,
                fontWeight: FontWeight.w800,
                color: AppColors.textPrimary,
                letterSpacing: -0.5,
              ),
              children: [TextSpan(text: '👋')],
            ),
          ),
          const SizedBox(height: 2),
        
          const SizedBox(height: 16),
        
          const SizedBox(height: 12),
        
        ],
      ),
    );
  }

  Widget _buildAiSection() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
      child: GestureDetector(
      onTap: () => Navigator.pushNamed(
  context,
  AppRoutes.aiRecommendations,
),
        child: Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [Color(0xFF5B3FD4), Color(0xFF9B7EFD)],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(18),
          ),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'A-I Çizgisimi & Öneriler',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 15,
                        fontWeight: FontWeight.w700,
                        letterSpacing: -0.3,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      'Vücut tipine uygun stil önerileri',
                      style: TextStyle(
                        color: Colors.white.withOpacity(0.75),
                        fontSize: 12,
                      ),
                    ),
                  ],
                ),
              ),
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: Colors.white.withOpacity(0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.arrow_forward_ios_rounded,
                    color: Colors.white, size: 18),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTabBar() {
    return Padding(
      padding: const EdgeInsets.fromLTRB(20, 20, 20, 12),
      child: Row(
        children: [
          _Tab(
            label: 'Son Analizler',
            active: _tabIndex == 0,
            onTap: () => setState(() => _tabIndex = 0),
          ),
          const SizedBox(width: 8),
        
          const Spacer(),
          GestureDetector(
            onTap: () {
  Navigator.pushNamed(
    context,
    AppRoutes.allAnalysis,
  );
},
            child:  Text(
              ' ',
              style: TextStyle(
                fontSize: 12,
                color: AppColors.primary,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
        ],
      ),
    
    );
  }
}

class _Tab extends StatelessWidget {
  final String label;
  final bool active;
  final VoidCallback onTap;

  const _Tab({required this.label, required this.active, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
        decoration: BoxDecoration(
          color: active ? AppColors.primary : Colors.white,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(
              color: active ? AppColors.primary : AppColors.divider),
        ),
        child: Text(
          label,
          style: TextStyle(
            fontSize: 13,
            fontWeight: FontWeight.w600,
            color: active ? Colors.white : AppColors.textSecondary,
          ),
        ),
      ),
    
    );
  }
  
}
