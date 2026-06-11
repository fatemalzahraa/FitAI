using System;
using Volo.Abp.Application.Dtos;

namespace FitAI.Yorumlar
{
    public class YorumDto : EntityDto<int>
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } 
        public string YorumMetni { get; set; }
        public int? Puan { get; set; }
        public bool NlpIslendi { get; set; }
    }
}