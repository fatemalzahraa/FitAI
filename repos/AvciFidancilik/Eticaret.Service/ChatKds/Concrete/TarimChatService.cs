using System.Collections.Concurrent;
using Eticaret.Core.DTOs.ChatBot;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.ChatKds.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eticaret.Service.ChatKds.Concrete
{
    public enum ChatState
    {
        MainMenu,
        FidanRecommendationStep_Category, FidanRecommendationStep_LandSize, FidanRecommendationStep_City, FidanRecommendationStep_Sunlight, FidanRecommendationStep_Watering, FidanRecommendationStep_Priority, FidanRecommendationStep_Finalize,
        ClimateStep_City, CareStep_ProductSelect, DiseaseStep_Symptom, SupportStep_Topic
    }

    public class FidanRecommendationRequest
    {
        public string CategoryName { get; set; } = "";
        public decimal LandSize { get; set; } = 0;
        public string City { get; set; } = "";
        public string ClimateType { get; set; } = "";
        public string Sunlight { get; set; } = "";
        public string Watering { get; set; } = "";
        public string Priority { get; set; } = "";
    }

    public class TarimChatService : ITarimChatService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<TarimChatService> _logger;
        private static readonly ConcurrentDictionary<string, ChatState> _states = new();
        private static readonly ConcurrentDictionary<string, FidanRecommendationRequest> _requests = new();

        // Sabit Linkler
        private const string WhatsAppLink = "https://wa.me/905327436282";
        private const string UrunlerimizLink = "/Home/Index#urunlerimiz";

        public TarimChatService(DatabaseContext ctx, ILogger<TarimChatService> logger)
        {
            _context = ctx;
            _logger = logger;
        }

        public async Task<ChatResponse> ProcessMessageAsync(int? userId, string? message, string? actionValue)
        {
            var key = userId?.ToString() ?? "guest_user_static_session";
            var response = new ChatResponse();
            string msg = message?.Trim() ?? "";
            string action = actionValue?.Trim() ?? "";

            // SCENARIO_MAIN geldiğinde veya mesaj/aksiyon boş olduğunda ana menüyü ver
            if (action == "SCENARIO_MAIN" || (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(msg)))
            {
                ResetUserSession(key);
                return await GetMainMenuResponse(response);
            }

            if (!_states.TryGetValue(key, out var state))
            {
                state = ChatState.MainMenu;
                _states[key] = state;
            }

            try
            {
                return state switch
                {
                    ChatState.MainMenu => await HandleMainMenu(key, action, response),
                    ChatState.FidanRecommendationStep_Category => await HandleFidanStep_Category(key, msg, response),
                    ChatState.FidanRecommendationStep_LandSize => await HandleFidanStep_LandSize(key, msg, response),
                    ChatState.FidanRecommendationStep_City => await HandleFidanStep_City(key, msg, response),
                    ChatState.FidanRecommendationStep_Sunlight => await HandleFidanStep_Sunlight(key, msg, response),
                    ChatState.FidanRecommendationStep_Watering => await HandleFidanStep_Watering(key, msg, response),
                    ChatState.FidanRecommendationStep_Priority => await HandleFidanStep_Priority(key, msg, response),
                    ChatState.FidanRecommendationStep_Finalize => await HandleFinalize(key, msg, response),
                    ChatState.ClimateStep_City => await HandleClimateStep_City(key, msg, response),
                    ChatState.CareStep_ProductSelect => await HandleCareStep_Product(key, msg, response),
                    ChatState.DiseaseStep_Symptom => await HandleDiseaseStep_Symptom(key, msg, response),
                    ChatState.SupportStep_Topic => await HandleSupportStep_Topic(key, msg, response),
                    _ => await GetMainMenuResponse(response)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "KDS Akış Hatası - Key: {Key}", key);
                ResetUserSession(key);
                return await GetMainMenuResponse(response);
            }
        }

        private async Task<ChatResponse> GetMainMenuResponse(ChatResponse res)
        {
            res.Message = "Merhaba! Avcı Fidancılık asistanına hoş geldiniz. Size nasıl yardımcı olabilirim? 🌿";
            res.Buttons = new List<string> { "🌳 Hangi fidan bana uygun?", "🌦️ İklime göre fidan öner", "🪴 Dikim & bakım rehberi", "🐛 Hastalık – zararlı tespiti", "📦 Kargo & iade destek" };
            res.ButtonActions = new List<string> { "SCENARIO_RECOMMEND", "SCENARIO_CLIMATE", "SCENARIO_CARE", "SCENARIO_DISEASE", "SCENARIO_SUPPORT" };
            return res;
        }

        private async Task<ChatResponse> HandleMainMenu(string key, string? action, ChatResponse res)
        {
            if (action == "SCENARIO_RECOMMEND")
            {
                _states[key] = ChatState.FidanRecommendationStep_Category;
                res.Message = "Hangi kategoride ürün için öneri almak istiyorsunuz?";
                res.Buttons = await _context.Categories.AsNoTracking().Select(c => c.Name).ToListAsync();
                res.ButtonActions = res.Buttons;
            }
            else if (action == "SCENARIO_CLIMATE")
            {
                _states[key] = ChatState.ClimateStep_City;
                res.Message = "İklim verisi için lütfen şehrinizi seçin:";
                res.Buttons = await _context.Cities.AsNoTracking().Select(c => c.Name).ToListAsync();
                res.ButtonActions = res.Buttons;
            }
            else if (action == "SCENARIO_CARE")
            {
                _states[key] = ChatState.CareStep_ProductSelect;
                res.Message = "Hangi ürünün bakım rehberini istersiniz?";
                res.Buttons = await _context.Products.AsNoTracking().Where(p => p.Stock > 0).Select(p => p.Name).Take(10).ToListAsync();
                res.ButtonActions = res.Buttons;
            }
            else if (action == "SCENARIO_DISEASE")
            {
                _states[key] = ChatState.DiseaseStep_Symptom;
                res.Message = "Bitkinizdeki ana sorun nedir?";
                res.Buttons = new List<string> { "Yaprak Sararması", "Kuruma", "Böceklenme" };
                res.ButtonActions = res.Buttons;
            }
            else if (action == "SCENARIO_SUPPORT")
            {
                _states[key] = ChatState.SupportStep_Topic;
                res.Message = "Destek almak istediğiniz konuyu seçin:";
                res.Buttons = new List<string> { "Kargo Sorgulama", "İade Talebi" };
                res.ButtonActions = res.Buttons;
            }
            else return await GetMainMenuResponse(res);
            return res;
        }

        private async Task<ChatResponse> HandleFinalize(string key, string msg, ChatResponse res)
        {
            if (!_requests.TryGetValue(key, out var req)) return await GetMainMenuResponse(res);

            var profiles = await _context.ProductAgricultureProfiles
                .Include(x => x.Product)
                .Include(x => x.Category)
                .Where(x => x.Category.Name == req.CategoryName && x.Product.Stock > 0)
                .ToListAsync();

            var bestMatch = profiles.Select(p => new { Profile = p, Score = (p.SuitableClimates != null && p.SuitableClimates.Contains(req.ClimateType) ? 3 : 0) + (p.SunlightNeeds == req.Sunlight ? 2 : 0) })
                .OrderByDescending(x => x.Score).FirstOrDefault();

            if (bestMatch != null)
            {
                var p = bestMatch.Profile;
                double spacing = p.MinSpacingCm > 0 ? (double)p.MinSpacingCm / 100.0 : 4.0;
                int totalTrees = (int)((double)req.LandSize * (1000.0 / (spacing * spacing)));

                res.Message = $"🌟 **Önerimiz:** {p.Product?.Name}\n\n🚜 {req.LandSize} dönüm için yaklaşık **{totalTrees} adet** fidan gereklidir.\n📏 Önerilen dikim mesafesi: {spacing} metre.";

                res.Buttons = new List<string> { "🛒 Ürünleri İncele", "📲 WhatsApp Destek", "🏠 Ana Menü" };
                res.ButtonActions = new List<string> { UrunlerimizLink, WhatsAppLink, "SCENARIO_MAIN" };
            }
            ResetUserSession(key);
            return res;
        }

        private async Task<ChatResponse> HandleClimateStep_City(string key, string msg, ChatResponse res)
        {
            var city = await _context.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Name.ToLower() == msg.ToLower());
            if (city == null) return await GetMainMenuResponse(res);

            var profiles = await _context.ProductAgricultureProfiles
                .Include(x => x.Product)
                .Where(p => p.SuitableClimates != null && p.SuitableClimates.Contains(city.ClimateType))
                .Take(3)
                .ToListAsync();

            if (profiles.Any())
            {
                var productList = string.Join("\n• ", profiles.Select(p => p.Product.Name));
                res.Message = $"🌦️ **{city.Name}** iklimine en uygun fidan önerilerimiz:\n\n• {productList}\n\n💡 Bu türler bölgenin iklim koşullarına dayanıklıdır.";
            }
            else
            {
                res.Message = $"😔 Maalesef **{city.Name}** iklimine uygun bir fidan kaydı bulamadım.";
            }

            res.Buttons = new List<string> { "🛒 Ürünleri İncele", "🏠 Ana Menü" };
            res.ButtonActions = new List<string> { UrunlerimizLink, "SCENARIO_MAIN" };
            ResetUserSession(key);
            return res;
        }

        private async Task<ChatResponse> HandleCareStep_Product(string key, string msg, ChatResponse res)
        {
            var p = await _context.ProductAgricultureProfiles
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Product.Name.ToLower() == msg.ToLower());

            if (p != null)
            {
                res.Message = $"🪴 **{p.Product.Name} Dikim & Bakım Rehberi**\n\n" +
                              $"📏 **Dikim Mesafesi:** {(p.MinSpacingCm > 0 ? p.MinSpacingCm + " cm" : "Belirtilmemiş")}\n" +
                              $"☀️ **Güneş İhtiyacı:** {p.SunlightNeeds ?? "Belirtilmemiş"}\n" +
                              $"💧 **Sulama:** {p.WaterNeeds ?? "Belirtilmemiş"}\n" +
                              $"🌦️ **Uygun İklim:** {p.SuitableClimates ?? "Tüm Bölgeler"}";
            }
            else
            {
                res.Message = "Bu ürün için bakım bilgisi bulunamadı.";
            }

            res.Buttons = new List<string> { "🏠 Ana Menü" };
            res.ButtonActions = new List<string> { "SCENARIO_MAIN" };
            ResetUserSession(key);
            return res;
        }

        private async Task<ChatResponse> HandleDiseaseStep_Symptom(string key, string msg, ChatResponse res)
        {
            string diagnosis = msg switch
            {
                "Yaprak Sararması" => "%80 demir eksikliği veya yanlış sulama olabilir.",
                "Kuruma" => "%75 kök çürüklüğü veya susuzluk stresi olabilir.",
                "Böceklenme" => "%85 yaprak biti veya zararlı istilası olabilir.",
                _ => "tanımlanamayan bir stres faktörü olabilir."
            };

            res.Message = $"🔍 **Ön Teşhis:** {diagnosis}\n\n" +
                          $"🧪 **İlk Öneri:** Bitkinin etkilenen kısımlarını gözlemleyin ve sulama düzenini kontrol edin.\n\n" +
                          $"⚠️ **Uyarı:** Kesin teşhis ve müdahale için lütfen ziraat mühendisine danışın.";

            res.Buttons = new List<string> { "📲 Uzmanla Görüş (WhatsApp)", "🏠 Ana Menü" };
            res.ButtonActions = new List<string> { WhatsAppLink, "SCENARIO_MAIN" };
            ResetUserSession(key);
            return res;
        }

        private async Task<ChatResponse> HandleFidanStep_Category(string key, string msg, ChatResponse res) { _requests[key] = new FidanRecommendationRequest { CategoryName = msg }; _states[key] = ChatState.FidanRecommendationStep_LandSize; res.Message = $"✅ **{msg}** seçildi. Kaç dönümlük bir araziye dikim yapacaksınız?"; return res; }
        private async Task<ChatResponse> HandleFidanStep_LandSize(string key, string msg, ChatResponse res) { if (!_requests.TryGetValue(key, out var req)) return res; req.LandSize = decimal.TryParse(msg, out decimal size) ? size : 1; _states[key] = ChatState.FidanRecommendationStep_City; res.Message = "Hangi şehirde dikim yapacaksınız?"; res.Buttons = await _context.Cities.AsNoTracking().Select(c => c.Name).ToListAsync(); res.ButtonActions = res.Buttons; return res; }
        private async Task<ChatResponse> HandleFidanStep_City(string key, string msg, ChatResponse res) { if (!_requests.TryGetValue(key, out var req)) return res; var city = await _context.Cities.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == msg.ToLower()); req.City = city?.Name ?? msg; req.ClimateType = city?.ClimateType ?? ""; _states[key] = ChatState.FidanRecommendationStep_Sunlight; res.Message = "Dikim alanınız ne kadar güneş alıyor?"; res.Buttons = new List<string> { "Tam güneş", "Yarı gölge", "Gölge" }; res.ButtonActions = res.Buttons; return res; }
        private Task<ChatResponse> HandleFidanStep_Sunlight(string key, string msg, ChatResponse res) { if (_requests.TryGetValue(key, out var req)) req.Sunlight = msg; _states[key] = ChatState.FidanRecommendationStep_Watering; res.Message = "Sulama imkanınız?"; res.Buttons = new List<string> { "Sık", "Orta", "Az" }; res.ButtonActions = res.Buttons; return Task.FromResult(res); }
        private Task<ChatResponse> HandleFidanStep_Watering(string key, string msg, ChatResponse res) { if (_requests.TryGetValue(key, out var req)) req.Watering = msg; _states[key] = ChatState.FidanRecommendationStep_Priority; res.Message = "Önceliğiniz?"; res.Buttons = new List<string> { "Meyve verimi", "Süs bitkisi" }; res.ButtonActions = res.Buttons; return Task.FromResult(res); }
        private Task<ChatResponse> HandleFidanStep_Priority(string key, string msg, ChatResponse res) { if (_requests.TryGetValue(key, out var req)) req.Priority = msg; _states[key] = ChatState.FidanRecommendationStep_Finalize; res.Message = "Analiz Tamamlandı!"; res.Buttons = new List<string> { "Sonuçları Göster" }; res.ButtonActions = res.Buttons; return Task.FromResult(res); }
        private async Task<ChatResponse> HandleSupportStep_Topic(string key, string msg, ChatResponse res) { res.Message = "Talebiniz alınmıştır. En kısa sürede size dönüş yapılacaktır."; res.Buttons = new List<string> { "🏠 Ana Menü" }; res.ButtonActions = new List<string> { "SCENARIO_MAIN" }; ResetUserSession(key); return res; }
        private void ResetUserSession(string key) { _states.TryRemove(key, out _); _requests.TryRemove(key, out _); }
    }
}