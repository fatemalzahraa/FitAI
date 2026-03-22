using Eticaret.Core.DTOs.ChatBot;
using System.Threading.Tasks;

namespace Eticaret.Service.ChatKds.Abstract
{
    /// <summary>
    /// Tarım Karar Destek Sistemi (KDS) Chatbot Servis Arayüzü
    /// </summary>
    public interface ITarimChatService
    {
        /// <summary>
        /// Kullanıcıdan gelen mesajı veya buton aksiyonunu işleyerek uygun cevabı döner.
        /// </summary>
        /// <param name="userId">Giriş yapmış kullanıcının ID'si (Opsiyonel)</param>
        /// <param name="userMessage">Kullanıcının yazdığı metin</param>
        /// <param name="actionValue">Butonlardan gelen özel aksiyon değeri</param>
        /// <returns>ChatResponse (Mesaj, Butonlar ve Ürün Önerileri)</returns>
        Task<ChatResponse> ProcessMessageAsync(int? userId, string? userMessage, string? actionValue);
    }
}