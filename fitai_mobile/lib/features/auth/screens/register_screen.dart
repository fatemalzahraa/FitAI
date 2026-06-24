import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../../core/theme/app_theme.dart';
import '../../../core/constants/app_routes.dart';
import '../../../core/widgets/common_widgets.dart';
import 'package:dio/dio.dart';
import '../../../core/services/api_service.dart';
class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  // =========================================================
  // CONTROLLERS
  // =========================================================

  final _nameCtrl = TextEditingController();

  final _emailCtrl = TextEditingController();

  final _passCtrl = TextEditingController();

  final _confirmPassCtrl = TextEditingController();

  // =========================================================
  // REPOSITORY
  // =========================================================

  // =========================================================
  // STATES
  // =========================================================

  bool _loading = false;

  bool _obscurePassword = true;

  bool _obscureConfirmPassword = true;

  // =========================================================
  // REGISTER FUNCTION
  // =========================================================

Future<void> _register() async {
  if (_nameCtrl.text.trim().isEmpty ||
      _emailCtrl.text.trim().isEmpty ||
      _passCtrl.text.trim().isEmpty ||
      _confirmPassCtrl.text.trim().isEmpty) {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Lütfen tüm alanları doldurun')),
    );
    return;
  }

  if (_passCtrl.text.trim() != _confirmPassCtrl.text.trim()) {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Şifreler eşleşmiyor')),
    );
    return;
  }

  try {
    setState(() => _loading = true);

    // 1. ÖNCE KAYIT
    await ApiService.dio.post(
      '/api/app/auth/register',
      data: {
        "userName": _nameCtrl.text.trim(),
        "email": _emailCtrl.text.trim(),
        "password": _passCtrl.text.trim(),
      },
    );

    // 2. SONRA OTOMATİK LOGIN
    final loginRes = await ApiService.dio.post(
  '/connect/token',
  data: {
    'grant_type': 'password',
    'username': _nameCtrl.text.trim(),
    'password': _passCtrl.text.trim(),
    'client_id': 'FitAI_Mobile',
    'client_secret': '1q2w3e*',
    'scope': 'openid profile email FitAI offline_access',
  },
  options: Options(contentType: Headers.formUrlEncodedContentType),
);
final token = loginRes.data['access_token'];

    // 3. TOKEN KAYDET
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('token', token);
    await prefs.setString('userName', _nameCtrl.text.trim());

    // 4. VÜCUT TİPİ SEÇİMİNE YÖNLENDİR
    Navigator.pushReplacementNamed(context, AppRoutes.bodyType);

} on DioException catch (e) {
  print("REGISTER HATA TİPİ: ${e.type}");
  print("REGISTER HATA MESAJ: ${e.message}");
  print("RESPONSE STATUS: ${e.response?.statusCode}");
  print("RESPONSE DATA: ${e.response?.data}");
  if (!mounted) return;
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(content: Text("Hata: ${e.type} | ${e.response?.data ?? e.message}")),
  );
} finally {
  setState(() => _loading = false);
}
}

  // =========================================================
  // DISPOSE
  // =========================================================

  @override
  void dispose() {
    _nameCtrl.dispose();

    _emailCtrl.dispose();

    _passCtrl.dispose();

    _confirmPassCtrl.dispose();

    super.dispose();
  }

  // =========================================================
  // UI
  // =========================================================

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

                // =========================================================
                // TOP BAR
                // =========================================================
                Row(
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

                        child: const Icon(
                          Icons.arrow_back_ios_rounded,
                          color: AppColors.textPrimary,
                          size: 18,
                        ),
                      ),
                    ),

                    const SizedBox(width: 16),

                    const Text(
                      'Hesap Oluştur',

                      style: TextStyle(
                        fontSize: 20,

                        fontWeight: FontWeight.w700,

                        color: AppColors.textPrimary,
                      ),
                    ),
                  ],
                ),

                const SizedBox(height: 32),

                // =========================================================
                // TITLE
                // =========================================================
                const Text(
                  'Merhaba! 👋',

                  style: TextStyle(
                    fontSize: 28,

                    fontWeight: FontWeight.w800,

                    color: AppColors.textPrimary,

                    letterSpacing: -0.5,
                  ),
                ),

                const SizedBox(height: 6),

                const Text(
                  'FitAI\'ye katıl, AI destekli\nalışveriş deneyimini yaşa.',

                  style: TextStyle(
                    fontSize: 15,

                    color: AppColors.textSecondary,

                    height: 1.4,
                  ),
                ),

                const SizedBox(height: 32),

                // =========================================================
                // NAME
                // =========================================================
                TextField(
                  controller: _nameCtrl,

                  decoration: const InputDecoration(
                    hintText: 'Ad Soyad',

                    prefixIcon: Icon(
                      Icons.person_outline_rounded,
                      color: AppColors.textHint,
                      size: 20,
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // =========================================================
                // EMAIL
                // =========================================================
                TextField(
                  controller: _emailCtrl,

                  keyboardType: TextInputType.emailAddress,

                  decoration: const InputDecoration(
                    hintText: 'E-mail adresi',

                    prefixIcon: Icon(
                      Icons.email_outlined,
                      color: AppColors.textHint,
                      size: 20,
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // =========================================================
                // PASSWORD
                // =========================================================
                TextField(
                  controller: _passCtrl,

                  obscureText: _obscurePassword,

                  decoration: InputDecoration(
                    hintText: 'Şifre',

                    prefixIcon: const Icon(
                      Icons.lock_outline_rounded,
                      color: AppColors.textHint,
                      size: 20,
                    ),

                    suffixIcon: GestureDetector(
                      onTap: () {
                        setState(() {
                          _obscurePassword = !_obscurePassword;
                        });
                      },

                      child: Icon(
                        _obscurePassword
                            ? Icons.visibility_off_outlined
                            : Icons.visibility_outlined,

                        color: AppColors.textHint,

                        size: 20,
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 12),

                // =========================================================
                // CONFIRM PASSWORD
                // =========================================================
                TextField(
                  controller: _confirmPassCtrl,

                  obscureText: _obscureConfirmPassword,

                  decoration: InputDecoration(
                    hintText: 'Şifre Tekrarı',

                    prefixIcon: const Icon(
                      Icons.lock_outline_rounded,
                      color: AppColors.textHint,
                      size: 20,
                    ),

                    suffixIcon: GestureDetector(
                      onTap: () {
                        setState(() {
                          _obscureConfirmPassword = !_obscureConfirmPassword;
                        });
                      },

                      child: Icon(
                        _obscureConfirmPassword
                            ? Icons.visibility_off_outlined
                            : Icons.visibility_outlined,

                        color: AppColors.textHint,

                        size: 20,
                      ),
                    ),
                  ),
                ),

                const SizedBox(height: 24),

                // =========================================================
                // REGISTER BUTTON
                // =========================================================
                GradientButton(
                  text: _loading ? 'Yükleniyor...' : 'Kayıt Ol',

                  onTap: _loading ? null : _register,
                ),

                const SizedBox(height: 24),

                // =========================================================
                // LOGIN REDIRECT
                // =========================================================
                Center(
                  child: GestureDetector(
                    onTap: () => Navigator.pop(context),

                    child: RichText(
                      text: const TextSpan(
                        text: 'Zaten hesabın var mı? ',

                        style: TextStyle(
                          color: AppColors.textSecondary,

                          fontSize: 13,
                        ),

                        children: [
                          TextSpan(
                            text: 'Giriş Yap',

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
