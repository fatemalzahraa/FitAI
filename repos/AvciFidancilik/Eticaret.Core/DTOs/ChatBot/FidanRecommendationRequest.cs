using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Core.DTOs.ChatBot
{
    public class FidanRecommendationRequest
    {
        public string CategoryName { get; set; } = "";
        public decimal LandSize { get; set; }
        // Hata buradaydı, bu satırı ekleyin:
        public string City { get; set; } = "";
        public string ClimateType { get; set; } = "";
        public string Sunlight { get; set; } = "";
        public string Watering { get; set; } = "";
        public string Priority { get; set; } = "";
    }

}
