using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Core.DTOs.ChatBot
{
    public class ChatOption
    {
        // Butonun hangi senaryoya ait olduğu (Örn: "FidanOnerisi")
        public string Scenario { get; set; } = string.Empty;

        // Kullanıcının butonda gördüğü yazı (Örn: "Meyve Fidanı")
        public string ButtonText { get; set; } = string.Empty;

        // Arka planda sunucuya gönderilen değer (Örn: "Category_Fruit")
        public string ActionValue { get; set; } = string.Empty;
    }
}
