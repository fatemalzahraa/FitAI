using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AvciFidancilik.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(DatabaseContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================
        // TEK GİRİŞ SAYFASI
        // =====================
        [HttpGet]
        public IActionResult SıgnIn(bool isAdminLogin = false)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                ViewBag.ErrorType = TempData["ErrorType"];
                ViewBag.UserName = TempData["UserName"];
            }

            ViewBag.IsAdminLogin = isAdminLogin;
            ViewBag.PageTitle = isAdminLogin ? "Admin Paneli Girişi" : "Müşteri Girişi";
            ViewBag.ModeClass = isAdminLogin ? "admin-mode" : "customer-mode";

            return View();
        }

        // =====================
        // ESKİ ADMIN GİRİŞ SAYFASI
        // =====================
        [HttpGet]
        public IActionResult AdminSignIn()
        {
            return RedirectToAction("SıgnIn", new { isAdminLogin = true });
        }

        // =====================
        // LOGIN ACTION - NULL REFERENCE DÜZELTMELERİ
        // =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string userNameOrEmail,
            string password,
            bool rememberMe = false,
            bool isAdminLogin = false)
        {
            try
            {
                // 1. VALİDASYON
                if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["ErrorMessage"] = "Lütfen kullanıcı adı ve şifre alanlarını doldurun";
                    TempData["ErrorType"] = "empty_fields";
                    TempData["UserName"] = userNameOrEmail;
                    return RedirectToAction("SıgnIn", new { isAdminLogin });
                }

                // 2. KULLANICI BUL
                var user = await _context.AppUsers
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(x =>
                        (x.UserName == userNameOrEmail || x.Email == userNameOrEmail) &&
                        x.IsActive
                    );

                if (user == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı: {userNameOrEmail}");
                    TempData["ErrorMessage"] = "Kullanıcı adı veya e-posta adresi bulunamadı";
                    TempData["ErrorType"] = "user_not_found";
                    TempData["UserName"] = userNameOrEmail;
                    return RedirectToAction("SıgnIn", new { isAdminLogin });
                }

                // 3. ŞİFRE KONTROLÜ - NULL REFERENCE DÜZELTMESİ
                bool isPasswordValid = false;

                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    // BCrypt kontrolü
                    if (user.PasswordHash.StartsWith("$2"))
                    {
                        isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                    }
                    else
                    {
                        // Eski plain text kontrol
                        isPasswordValid = (user.PasswordHash == password);
                    }
                }

                // 4. ŞİFRE YANLIŞSA
                if (!isPasswordValid)
                {
                    _logger.LogWarning($"Şifre yanlış - Kullanıcı: {userNameOrEmail}");
                    TempData["ErrorMessage"] = "Girdiğiniz şifre yanlış. Lütfen tekrar deneyin.";
                    TempData["ErrorType"] = "wrong_password";
                    TempData["UserName"] = userNameOrEmail;
                    return RedirectToAction("SıgnIn", new { isAdminLogin });
                }

                // 5. ADMIN KONTROLÜ - NULL REFERENCE DÜZELTMESİ
                bool hasAdminRoleFromTable = false;
                if (user.UserRoles != null)
                {
                    hasAdminRoleFromTable = user.UserRoles
                        .Any(ur => ur.Role != null &&
                               ur.Role.Name != null &&
                               ur.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));
                }

                bool hasAdminFromField = user.IsAdmin;
                bool isAdmin = hasAdminRoleFromTable || hasAdminFromField;

                // 6. ADMIN GİRİŞİ ÖZEL KONTROL
                if (isAdminLogin && !isAdmin)
                {
                    _logger.LogWarning($"Admin yetkisi olmayan kullanıcı admin girişi denedi: {userNameOrEmail}");
                    TempData["ErrorMessage"] = "Bu sayfaya erişim için admin yetkisi gereklidir";
                    TempData["ErrorType"] = "not_admin";
                    TempData["UserName"] = userNameOrEmail;
                    return RedirectToAction("SıgnIn", new { isAdminLogin = true });
                }

                // 7. CLAIMS OLUŞTUR - NULL REFERENCE DÜZELTMESİ
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
                    new Claim("FullName", $"{user.Name} {user.SurName}"),
                    new Claim("UserId", user.Id.ToString())
                };

                // 8. AUTHENTICATION COOKIE
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                );

                // 9. SESSION VERİLERİ
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.UserName ?? string.Empty);
                HttpContext.Session.SetString("IsAdmin", isAdmin.ToString().ToLower());

                // 10. BAŞARILI GİRİŞ LOG
                _logger.LogInformation($"Başarılı giriş: {user.UserName} ({(isAdmin ? "Admin" : "User")})");

                // 11. YÖNLENDİRME
                if (isAdmin)
                {
                    return RedirectToAction("Index", "Main", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login işleminde hata");
                TempData["ErrorMessage"] = "Sistem hatası oluştu. Lütfen daha sonra tekrar deneyin.";
                TempData["ErrorType"] = "system_error";
                TempData["UserName"] = userNameOrEmail;
                return RedirectToAction("SıgnIn", new { isAdminLogin });
            }
        }

        // =====================
        // ŞİFRE DEĞİŞTİRME
        // =====================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                // NULL REFERENCE DÜZELTMESİ
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    TempData["PasswordError"] = "Geçersiz kullanıcı oturumu";
                    return RedirectToAction("Profile", "Account");
                }

                var user = await _context.AppUsers.FindAsync(userId);
                if (user == null)
                {
                    TempData["PasswordError"] = "Kullanıcı bulunamadı";
                    return RedirectToAction("Profile", "Account");
                }

                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    TempData["PasswordError"] = "Şifre en az 6 karakter olmalıdır";
                    return RedirectToAction("Profile", "Account");
                }

                if (newPassword != confirmPassword)
                {
                    TempData["PasswordError"] = "Yeni şifreler uyuşmuyor";
                    return RedirectToAction("Profile", "Account");
                }

                // Mevcut şifreyi kontrol et - NULL REFERENCE DÜZELTMESİ
                bool isCurrentValid = false;
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    if (user.PasswordHash.StartsWith("$2"))
                    {
                        isCurrentValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
                    }
                    else
                    {
                        isCurrentValid = (user.PasswordHash == currentPassword);
                    }
                }

                if (!isCurrentValid)
                {
                    TempData["PasswordError"] = "Mevcut şifre yanlış";
                    return RedirectToAction("Profile", "Account");
                }

                // Yeni şifreyi hash'le
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, 11);
                await _context.SaveChangesAsync();

                TempData["PasswordSuccess"] = "Şifreniz başarıyla değiştirildi";
                return RedirectToAction("Profile", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre değiştirme hatası");
                TempData["PasswordError"] = "Şifre değiştirilirken hata oluştu";
                return RedirectToAction("Profile", "Account");
            }
        }

        // =====================
        // PROFİL SAYFASI
        // =====================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // NULL REFERENCE DÜZELTMESİ
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Logout");
            }

            var user = await _context.AppUsers
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Logout");
            }

            return View(user);
        }

        // =====================
        // LOGOUT
        // =====================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            _logger.LogInformation($"Çıkış yapıldı");

            TempData["LogoutMessage"] = "Başarıyla çıkış yaptınız. Tekrar bekleriz!";
            return RedirectToAction("SıgnIn");
        }

        // =====================
        // ACCESS DENIED
        // =====================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // =====================
        // ADMIN ŞİFRE SIFIRLAMA
        // =====================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminResetPassword(int userId, string newPassword = "")
        {
            try
            {
                // NULL REFERENCE DÜZELTMESİ
                var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int _))
                {
                    return Json(new { success = false, message = "Geçersiz oturum" });
                }

                var targetUser = await _context.AppUsers.FindAsync(userId);
                if (targetUser == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // Eğer yeni şifre belirtilmemişse, rastgele şifre oluştur
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    newPassword = GenerateRandomPassword();
                }

                // Yeni BCrypt hash oluştur
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, 11);
                targetUser.PasswordHash = newHash;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Admin şifre sıfırlama: {User.Identity?.Name} -> {targetUser.UserName}");

                return Json(new
                {
                    success = true,
                    message = "Şifre başarıyla sıfırlandı",
                    newPassword = newPassword
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin şifre sıfırlama hatası");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }
    }
}