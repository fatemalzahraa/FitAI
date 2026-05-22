using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FitAI.Domain.Products;

namespace FitAI.Yorumlar
{
    public class YorumGetListInput : PagedAndSortedResultRequestDto
    {
        public int? UrunId { get; set; }
        public int? MagazaId { get; set; }
    }

    public class YorumAppService :
        CrudAppService<Yorum, YorumDto, int, YorumGetListInput, CreateUpdateYorumDto>,
        IYorumAppService
    {
        public YorumAppService(IRepository<Yorum, int> repository) : base(repository) { }

        // GET /api/app/yorum?urunId=5&maxResultCount=100
// GET /api/app/yorum?urunId=5&maxResultCount=100
        protected override async Task<IQueryable<Yorum>> CreateFilteredQueryAsync(YorumGetListInput input)
        {
            // 1. Önce baz sorguyu alıyoruz ve açıkça IQueryable<Yorum> tipine atıyoruz.
            // Bu sayede .Where eklesek de tip karmaşası yaşanmıyor.
            IQueryable<Yorum> query = await base.CreateFilteredQueryAsync(input);

            // 2. İlişkili tabloları (.Include) bağlıyoruz.
            query = query.Include(y => y.Urun)
                         .Include(y => y.Magaza);

            // 3. Filtreleme koşullarını uyguluyoruz (Artık buradaki 32 ve 35. satırlar hata vermeyecek!)
            if (input.UrunId.HasValue)
                query = query.Where(y => y.UrunId == input.UrunId.Value);

            if (input.MagazaId.HasValue)
                query = query.Where(y => y.MagazaId == input.MagazaId.Value);

            return query;
        }
        public override async Task<YorumDto> GetAsync(int id)
        {
            var queryable = await Repository.WithDetailsAsync(y => y.Urun, y => y.Magaza);
            var yorum = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(y => y.Id == id));
            if (yorum == null) throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Yorum), id);
            return ObjectMapper.Map<Yorum, YorumDto>(yorum);
        }

        public async Task<List<YorumDto>> GetListByUrunIdAsync(int urunId)
        {
            var queryable = await Repository.WithDetailsAsync(y => y.Urun, y => y.Magaza);
            var yorumlar = await queryable
                .Where(y => y.UrunId == urunId)
                .ToListAsync();
            return ObjectMapper.Map<List<Yorum>, List<YorumDto>>(yorumlar);
        }

        // POST /api/app/yorum/{id}/trigger-nlp
        public async Task<YorumDto> TriggerNlpAsync(int id)
        {
            var yorum = await Repository.GetAsync(id);

            var metin = (yorum.YorumMetni ?? "").ToLowerInvariant();

            if (metin.Contains("memnun") || metin.Contains("harika") ||
                metin.Contains("güzel")  || metin.Contains("iyi")    ||
                metin.Contains("rahat")  || metin.Contains("kaliteli"))
            {
                yorum.Duygu      = "Olumlu";
                yorum.GuvenSkoru = 0.88;
            }
            else if (metin.Contains("kötü")  || metin.Contains("iade")  ||
                     metin.Contains("dar")   || metin.Contains("sorun") ||
                     metin.Contains("beklentimi karşılamadı"))
            {
                yorum.Duygu      = "Olumsuz";
                yorum.GuvenSkoru = 0.82;
            }
            else
            {
                yorum.Duygu      = "Nötr";
                yorum.GuvenSkoru = 0.65;
            }

            yorum.NlpIslendi = true;
            await Repository.UpdateAsync(yorum, autoSave: true);

            var queryable = await Repository.WithDetailsAsync(y => y.Urun, y => y.Magaza);
            var guncel = await queryable.FirstOrDefaultAsync(y => y.Id == id);
            return ObjectMapper.Map<Yorum, YorumDto>(guncel ?? yorum);
        }
    }
}