namespace Eticaret.Core.Entities
{
    public class ProductAgricultureProfile : IEntity
    {
        public int Id { get; set; }

        // FK İlişkileri
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // Hata buradaydı, null! ekledik.

        // Kategori bağlantısı chatbot için kritiktir
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // 1️⃣ ÖNERİ & KONUM
        public string SuitableClimates { get; set; } = string.Empty;
        public string SuitableSoilTypes { get; set; } = string.Empty;
        public string SunlightNeeds { get; set; } = string.Empty;
        public string WaterNeeds { get; set; } = string.Empty;

        // 2️⃣ VERİM HESAPLAMA & TİCARİ
        public int AverageYieldPerTreeKg { get; set; }
        public int HarvestEfficiencyScore { get; set; }
        public int FirstHarvestYear { get; set; }
        public decimal PhMin { get; set; } = 6.0m;
        public decimal PhMax { get; set; } = 8.0m;
        public bool FrostResistant { get; set; }

        // 3️⃣ DİKİM & BAKIM
        public int MinSpacingCm { get; set; }
        public int MaxSpacingCm { get; set; }
        public int PlantingDepthCm { get; set; }
        public string PlantingSeason { get; set; } = string.Empty;
        public string IrrigationInfo { get; set; } = string.Empty;
        public string FertilizationInfo { get; set; } = string.Empty;
        public string PruningInfo { get; set; } = string.Empty;

        // 4️⃣ HASTALIK & CHATBOT
        public string DiseaseSymptoms { get; set; } = string.Empty;
        public string DiagnosisAndSolution { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
    }
}