using Eticaret.Core.DTOs;
using System.Threading.Tasks;

namespace Eticaret.Service.Abstract
{
    public interface IAdminKdsService
    {
        // Tüm dashboard verilerini tek seferde döner
        Task<AdminKdsDto> GetDashboardSummaryAsync();

        // Stok ve Trend analizini asenkron yapar
        Task<AdminKdsDto> GetStockAnalyticsAsync();
    }
}