import 'package:flutter/material.dart';
import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';
import 'package:dio/dio.dart';
import '../../../core/services/api_service.dart';
import 'package:shared_preferences/shared_preferences.dart';
class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _emailCtrl = TextEditingController();
  final _passCtrl = TextEditingController();
  bool _obscure = true;
  bool _loading = false;
Future<void> _login() async {
  if (_emailCtrl.text.trim().isEmpty || _passCtrl.text.trim().isEmpty) {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Tüm alanları doldurun')),
    );
    return;
  }

  try {
    setState(() => _loading = true);

    final response = await ApiService.dio.post(
      '/connect/token',
      data: {
        'grant_type': 'password',
        'username': _emailCtrl.text.trim(),
        'password': _passCtrl.text.trim(),
        'client_id': 'FitAI_Mobile',
        'client_secret': '1q2w3e*',
        'scope': 'openid profile email FitAI offline_access',
      },
      options: Options(contentType: Headers.formUrlEncodedContentType),
    );

    final token = response.data['access_token'];
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('token', token);
    await prefs.setString('userName', _emailCtrl.text.trim());

    Navigator.pushReplacementNamed(context, AppRoutes.main);

  } on DioException catch (e) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(e.response?.data.toString() ?? 'Giriş başarısız')),
    );
  } finally {
    setState(() => _loading = false);
  }
}
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(gradient: AppColors.bgGradient),
        child: SafeArea(
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 20),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const FitAILogo(size: 22),
                    TextButton(
                      onPressed: () =>
                          Navigator.pushNamed(context, AppRoutes.register),
                      child: const Text(
                        'Kayıt Ol',
                        style: TextStyle(
                          color: AppColors.primary,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 40),

                // User Avatar Placeholder
                Center(
                  child: Column(
                    children: [
                      Container(
                        width: 72,
                        height: 72,
                        decoration: BoxDecoration(
                          color: AppColors.primary.withOpacity(0.1),
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(
                          Icons.person_rounded,
                          color: AppColors.primary,
                          size: 36,
                        ),
                      ),
                      const SizedBox(height: 12),
                      
                    ],
                  ),
                ),
                const SizedBox(height: 36),

                // Form
              TextField(
  controller: _emailCtrl,
  decoration: const InputDecoration(
    hintText: 'User Name',
    prefixIcon: Icon(
      Icons.person_outline_rounded,
      color: AppColors.textHint,
      size: 20,
    ),
  ),
),
                const SizedBox(height: 12),
                TextField(
                  controller: _passCtrl,
                  obscureText: _obscure,
                  decoration: InputDecoration(
                    hintText: 'Password',
                    prefixIcon: const Icon(Icons.lock_outline_rounded, color: AppColors.textHint, size: 20),
                    suffixIcon: GestureDetector(
                      onTap: () => setState(() => _obscure = !_obscure),
                      child: Icon(
                        _obscure ? Icons.visibility_off_outlined : Icons.visibility_outlined,
                        color: AppColors.textHint,
                        size: 20,
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 8),
                Align(
                  alignment: Alignment.centerRight,
                  child: TextButton(
                    onPressed: () {},
                    child: const Text(
                      'Şifremi Unuttum?',
                      style: TextStyle(color: AppColors.textSecondary, fontSize: 13),
                    ),
                  ),
                ),
                const SizedBox(height: 16),
                GradientButton(
  text: _loading ? 'Yükleniyor...' : 'Giriş Yap',
  onTap: _loading ? null : _login,
),
                const SizedBox(height: 24),

                // Divider
                Row(
                  children: [
                    const Expanded(child: Divider(color: AppColors.divider)),
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 12),
                      child: Text(
                        'veya',
                        style: TextStyle(color: AppColors.textHint, fontSize: 13),
                      ),
                    ),
                    const Expanded(child: Divider(color: AppColors.divider)),
                  ],
                ),
                const SizedBox(height: 20),

                // Social buttons
                Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    _SocialBtn(icon: Icons.facebook_rounded, color: const Color(0xFF1877F2), onTap: () {}),
                    const SizedBox(width: 16),
                    _SocialBtn(icon: Icons.close_rounded, color: Colors.black87, onTap: () {}),
                    const SizedBox(width: 16),
                    _SocialBtn(icon: Icons.chat_rounded, color: const Color(0xFF1DA1F2), onTap: () {}),
                  ],
                ),
                const SizedBox(height: 32),
                Center(
                  child: GestureDetector(
                    onTap: () => Navigator.pushNamed(context, AppRoutes.register),
                    child: RichText(
                      text: const TextSpan(
                        text: 'Henüz hesabın yok mu? ',
                        style: TextStyle(color: AppColors.textSecondary, fontSize: 13),
                        children: [
                          TextSpan(
                            text: 'Kayıt Ol',
                            style: TextStyle(
                              color: AppColors.primary,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 24),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _SocialBtn extends StatelessWidget {
  final IconData icon;
  final Color color;
  final VoidCallback onTap;

  const _SocialBtn({required this.icon, required this.color, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 52,
        height: 52,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: AppColors.divider),
        ),
        child: Icon(icon, color: color, size: 24),
      ),
    );
  }
}
