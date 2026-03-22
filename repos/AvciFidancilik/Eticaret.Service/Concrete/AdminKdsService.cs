using Eticaret.Core.DTOs;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eticaret.Service.Concrete
{
    public class AdminKdsService : IAdminKdsService
    {
        private readonly DatabaseContext _context;

        public AdminKdsService(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<AdminKdsDto> GetDashboardSummaryAsync()
        {
            var dto = await GetStockAnalyticsAsync();

            // 1. Toplam Terk Edilmiş Sepet Sayısı
            dto.AbandonedCartCount = await _context.CartItems.Select(ci => ci.CartId).Distinct().CountAsync();

            // 2. DETAY ANALİZ: Hangi ürün sepette ne kadar bekliyor?
            // Not: AdminKdsDto içinde AbandonedDetails isminde bir liste olduğunu varsayıyoruz.
            dto.AbandonedDetails = await _context.CartItems
                .Include(ci => ci.Product)
                .GroupBy(ci => new { ci.ProductId, ci.Product.Name, ci.Product.Price })
                .Select(g => new AbandonedCartDetailDto
                {
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                    Price = g.Key.Price,
                    Total = g.Sum(x => (decimal)x.Quantity * g.Key.Price)
                })
                .OrderByDescending(x => x.Quantity)
                .ToListAsync();

            // Diğer kodlar (SeasonalSuggestions ve ActiveCustomers) aynı kalıyor...
            return dto;
        }

        public async Task<AdminKdsDto> GetStockAnalyticsAsync()
        {
            var dto = new AdminKdsDto();
            var today = DateTime.UtcNow;
            var thirtyDaysAgo = today.AddDays(-30);
            var sixtyDaysAgo = today.AddDays(-60);

            var salesData = await _context.OrderItems
                .Where(oi => oi.Order != null && oi.Order.OrderDate >= sixtyDaysAgo)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.Stock })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.Name,
                    g.Key.Stock,
                    CurrentSales = g.Where(x => x.Order.OrderDate >= thirtyDaysAgo).Sum(x => x.Quantity),
                    PrevSales = g.Where(x => x.Order.OrderDate < thirtyDaysAgo).Sum(x => x.Quantity),
                    CurrentRevenue = g.Where(x => x.Order.OrderDate >= thirtyDaysAgo).Sum(x => (decimal)x.Quantity * x.UnitPrice)
                })
                .ToListAsync();

            dto.MonthlySales = salesData.Sum(x => x.CurrentRevenue);
            var prevTotal = salesData.Sum(x => x.PrevSales);
            var currentTotal = salesData.Sum(x => x.CurrentSales);
            if (prevTotal > 0) dto.SalesGrowth = ((double)(currentTotal - prevTotal) / prevTotal) * 100;

            foreach (var item in salesData)
            {
                decimal dailyVel = (decimal)item.CurrentSales / 30m;
                int? estDays = dailyVel > 0 ? (int)Math.Floor(item.Stock / dailyVel) : null;
                double trend = item.PrevSales > 0 ? ((double)(item.CurrentSales - item.PrevSales) / item.PrevSales) * 100 : (item.CurrentSales > 0 ? 100 : 0);

                // Eşik değeri 50 olarak güncellendi veya hızlı tükenme/trend kontrolü
                if (item.Stock < 50 || (estDays.HasValue && estDays <= 7) || trend >= 40)
                {
                    var warning = new StockWarningDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Name,
                        CurrentStock = item.Stock,
                        EstimatedDaysToExhaust = estDays,
                        SalesTrendPercentage = Math.Round(trend, 2),

                        // 🚀 ÖNCELİK GÜNCELLEMESİ: 
                        // Önce hayati olan kritik duruma bakıyoruz.
                        // Stok 50'nin altındaysa veya 3 gün içinde bitecekse -> Critical
                        AlertLevel = (item.Stock < 50 || (estDays.HasValue && estDays <= 3)) ? "Critical" :
                                     (estDays.HasValue && estDays <= 7) ? "High" :
                                     (trend >= 40) ? "Success" : "Medium"
                    };

                    dto.StockWarnings.Add(warning);

                    if (warning.AlertLevel == "Critical") dto.CriticalStockCount++;
                    if (item.Stock < 50) dto.LowStockCount++;
                }
            }
            return dto;
        }


        private async Task<List<string>> GetSeasonalRecommendationsAsync()
        {
            var res = new List<string>();
            var month = DateTime.Now.Month;
            if (month >= 3 && month <= 5) res.Add("🌱 Bahar dikim sezonu başladı.");
            if (month == 12 || month <= 2) res.Add("❄️ Kış bakımı ve sera ürünlerine odaklanın.");
            return res;
        }
    }
}