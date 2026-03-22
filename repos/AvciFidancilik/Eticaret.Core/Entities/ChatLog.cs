using Eticaret.Core.Entities;

namespace Eticaret.Data.Entities
{
    public class ChatLog : IEntity
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Kullanıcı login ise bağla
        public string KullaniciMesaj { get; set; } = string.Empty;
        public string BotCevap { get; set; } = string.Empty;

        // --- GÜNCELLEME: Durum Takibi ---
        public string? CurrentScenario { get; set; } // Örn: "FidanOnerisi"
        public int StepIndex { get; set; } // Kaçıncı sorudayız?
        public string? RawData { get; set; } // JSON formatında geçici veriler (dönüm, şehir vb.)

        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }
}