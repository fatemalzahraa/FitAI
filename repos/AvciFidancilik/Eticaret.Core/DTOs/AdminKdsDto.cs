using System;
using System.Collections.Generic;

namespace Eticaret.Core.DTOs
{
    public class AdminKdsDto
    {
        // 1. GÜN: Sepet Analizi
        public int AbandonedCartCount { get; set; }

        // 4. GÜN: Mevsimsel Tavsiyeler
        public List<string> SeasonalSuggestions { get; set; } = new List<string>();

        // 2. & 3. GÜN: Stok ve Trend Uyarıları
        public List<StockWarningDto> StockWarnings { get; set; } = new List<StockWarningDto>();

        // 5. GÜN: Yeni KPI Metrikleri (Gelişmiş Dashboard için)
        public decimal MonthlySales { get; set; }
        public double SalesGrowth { get; set; }
        public int ActiveCustomers { get; set; }
        public int LowStockCount { get; set; }
        public int CriticalStockCount { get; set; }
        public List<AbandonedCartDetailDto> AbandonedDetails { get; set; } = new();
    }

    public class StockWarningDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int? EstimatedDaysToExhaust { get; set; } // Nullable: Satış yoksa "Bilinmiyor"
        public double SalesTrendPercentage { get; set; }
        public string AlertLevel { get; set; } = "Low"; // Success, Critical, High, Medium, Low
    }
}