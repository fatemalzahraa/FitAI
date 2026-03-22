using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvciFidancilik.WebUI.ViewComponents
{
    public class SliderViewComponent : ViewComponent
    {
        private readonly DatabaseContext _context;

        public SliderViewComponent(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Sadece aktif slider'ları çek, ID'ye göre sırala
            var sliders = await _context.Sliders
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(sliders);
        }
    }
}