using Eticaret.Core.Entities;

namespace Eticaret.Core.DTOs.ChatBot
{
    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;

        // Buton listesi
        public List<string>? Buttons { get; set; }

        // Ürün önerileri
        public List<Product>? SuggestedProducts { get; set; }

        // Senaryo bilgileri (zorunlu değil)
        public string? Scenario { get; set; }
        public string? ButtonText { get; set; }
        public string? ActionValue { get; set; }
        public List<string> ButtonActions { get; set; } = new();
    }
}
