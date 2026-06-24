import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/services/api_service.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  final ApiService _api = ApiService();

  String name = "";
  String email = "";
  String bodyType = "Unknown";

  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    loadProfile();
  }

  Future<void> loadProfile() async {
    final prefs = await SharedPreferences.getInstance();
    final savedName = prefs.getString('userName') ?? "";

    try {
      final res = await _api.getMyProfile();

      print("PROFILE RESPONSE:");
      print(res.data);

      final data = res.data;
      print(data);
print(data["name"]);
print(data["email"]);

      final userName = data["name"] ?? savedName;

// SharedPreferences'a kaydet
await prefs.setString('userName', userName);

await prefs.setString('bodyType', data["bodyType"] ?? "Armut");
setState(() {
  name = userName;
  email = data["email"] ?? "";
  bodyType = data["bodyType"] ?? "Unknown";
  isLoading = false;
});
    } catch (e) {
      setState(() {
        name = savedName;
        isLoading = false;
      });
    }
  }

  Color getBodyTypeColor() {
    switch (bodyType.toLowerCase()) {
      case "oval":
        return Colors.orange;
      case "rectangle":
        return Colors.blue;
      case "hourglass":
        return Colors.pink;
      case "triangle":
        return Colors.green;
      default:
        return AppColors.primary;
    }
  }

  Widget buildStatCard(
      String title,
      String value,
      IconData icon,
      ) {
    return Expanded(
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 6),
        padding: const EdgeInsets.all(18),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          boxShadow: [
            BoxShadow(
              blurRadius: 10,
              color: Colors.black.withOpacity(0.05),
            )
          ],
        ),
        child: Column(
          children: [
            Icon(
              icon,
              color: AppColors.primary,
              size: 28,
            ),
            const SizedBox(height: 10),
            Text(
              value,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w800,
              ),
            ),
            const SizedBox(height: 5),
            Text(
              title,
              style: TextStyle(
                color: Colors.grey.shade600,
                fontSize: 13,
              ),
            )
          ],
        ),
      ),
    );
  }

  Widget buildMenuTile({
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
    Color? iconColor,
  }) {
    return Container(
      margin: const EdgeInsets.symmetric(
        horizontal: 20,
        vertical: 6,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
      ),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor:
          (iconColor ?? AppColors.primary).withOpacity(0.12),
          child: Icon(
            icon,
            color: iconColor ?? AppColors.primary,
          ),
        ),
        title: Text(
          title,
          style: const TextStyle(
            fontWeight: FontWeight.w700,
          ),
        ),
        subtitle: Text(subtitle),
        trailing: const Icon(Icons.arrow_forward_ios_rounded),
        onTap: onTap,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Container(
        decoration: const BoxDecoration(
          gradient: AppColors.bgGradient,
        ),
        child: SafeArea(
          child: isLoading
              ? const Center(
            child: CircularProgressIndicator(),
          )
              : RefreshIndicator(
            onRefresh: loadProfile,
            child: SingleChildScrollView(
              physics: const BouncingScrollPhysics(),
              child: Column(
                children: [
                  const SizedBox(height: 20),

                  // HEADER
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 20),
                    child: Row(
                      mainAxisAlignment:
                      MainAxisAlignment.spaceBetween,
                      children: [
                        const Text(
                          'Profil',
                          style: TextStyle(
                            fontSize: 26,
                            fontWeight: FontWeight.w800,
                            color: AppColors.textPrimary,
                          ),
                        ),
                      
                      ],
                    ),
                  ),

                  const SizedBox(height: 20),

                  // PROFILE CARD
                  Container(
                    margin: const EdgeInsets.symmetric(
                        horizontal: 20),
                    padding: const EdgeInsets.all(22),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          AppColors.primary,
                          AppColors.primary.withOpacity(0.75),
                        ],
                      ),
                      borderRadius:
                      BorderRadius.circular(28),
                      boxShadow: [
                        BoxShadow(
                          blurRadius: 20,
                          offset: const Offset(0, 10),
                          color:
                          AppColors.primary.withOpacity(0.25),
                        )
                      ],
                    ),
                    child: Row(
                      children: [
                        CircleAvatar(
                          radius: 38,
                          backgroundColor: Colors.white,
                          child: Text(
                            name.isNotEmpty
                                ? name[0].toUpperCase()
                                : "U",
                            style: TextStyle(
                              color: AppColors.primary,
                              fontWeight: FontWeight.bold,
                              fontSize: 28,
                            ),
                          ),
                        ),

                        const SizedBox(width: 18),

                        Expanded(
                          child: Column(
                            crossAxisAlignment:
                            CrossAxisAlignment.start,
                            children: [
                              Text(
                                name,
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 22,
                                  fontWeight:
                                  FontWeight.bold,
                                ),
                              ),

                              const SizedBox(height: 6),

                              Text(
                                email,
                                style: const TextStyle(
                                  color: Colors.white70,
                                ),
                              ),

                              const SizedBox(height: 14),

                              Container(
                                padding:
                                const EdgeInsets.symmetric(
                                  horizontal: 14,
                                  vertical: 6,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.white,
                                  borderRadius:
                                  BorderRadius.circular(
                                      14),
                                ),
                                child: Text(
                                  bodyType,
                                  style: TextStyle(
                                    color:
                                    getBodyTypeColor(),
                                    fontWeight:
                                    FontWeight.w700,
                                  ),
                                ),
                              )
                            ],
                          ),
                        )
                      ],
                    ),
                  ),

                  const SizedBox(height: 28),

                  // STATS
                  

                  const SizedBox(height: 30),

                  // ACCOUNT
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 20),
                    child: Row(
                      children: const [
                        Text(
                          "Hesap",
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 12),

                  buildMenuTile(
                    icon: Icons.person_outline,
                    title: "Vücut Tipi",
                    subtitle:
                    "Vücut tipini düzenle",
                    onTap: () {
                      Navigator.pushNamed(
                        context,
                        AppRoutes.bodyType,
                      );
                    },
                  ),

                  buildMenuTile(
                    icon: Icons.favorite_border,
                    title: "Favoriler",
                    subtitle:
                    "Kaydettiğin kombinler",
                    onTap: () {},
                  ),

                  

                  

                  buildMenuTile(
                    icon: Icons.dark_mode_outlined,
                    title: "Tema",
                    subtitle:
                    "Karanlık mod ayarları",
                    onTap: () {},
                  ),

                

                  const SizedBox(height: 20),

                  // LOGOUT
                  buildMenuTile(
                    icon: Icons.logout_rounded,
                    iconColor: Colors.red,
                    title: "Çıkış Yap",
                    subtitle: "Hesaptan güvenli çıkış",
                    onTap: () async {
                      final prefs =
                      await SharedPreferences.getInstance();

                      await prefs.remove('token');

                      if (!mounted) return;

                      Navigator.pushReplacementNamed(
                        context,
                        AppRoutes.login,
                      );
                    },
                  ),

                  const SizedBox(height: 30),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}