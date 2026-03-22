using Eticaret.Service.ChatKds.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Eticaret.Core.DTOs.ChatBot;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AvciFidancilik.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ITarimChatService _chatService;

        public ChatController(ITarimChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Kullanıcıdan gelen mesajı veya buton aksiyonunu işler.
        /// </summary>
        [HttpPost("SendMessage")]
        [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            // 🧱 1. Boş İstek Kontrolü
            if (request == null || (string.IsNullOrWhiteSpace(request.Message) && string.IsNullOrWhiteSpace(request.ActionValue)))
            {
                return BadRequest("Mesaj veya aksiyon değeri boş olamaz.");
            }

            try
            {
                // 🔒 2. userId Yönetimi (State ve loglama için)
                int? userId = null;
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
                {
                    userId = parsedId;
                }

                // 🚀 3. Servis Çağrısı 
                // Önemli: Metot ismi ProcessMessageAsync olarak interface ile eşitlendi.
                // Parametreler: (userId, message, actionValue)
                var response = await _chatService.ProcessMessageAsync(
                    userId,
                    request.Message,
                    request.ActionValue
                );

                return Ok(response);
            }
            catch (Exception)
            {
                // Hata durumunda loglama mekanizması buraya eklenebilir.
                return StatusCode(500, "Chat servisi işlenirken beklenmedik bir hata oluştu.");
            }
        }
    }

    /// <summary>
    /// Chatbot istek modeli
    /// </summary>
    public class ChatRequest
    {
        public string? Message { get; set; } = string.Empty;
        public string? ActionValue { get; set; } = string.Empty;
    }
}