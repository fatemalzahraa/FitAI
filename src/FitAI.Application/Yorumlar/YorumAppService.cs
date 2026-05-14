using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FitAI.Domain.Products;
using FitAI.Yorumlar;

namespace FitAI.Yorumlar
{
    public class YorumAppService :
        CrudAppService<Yorum, YorumDto, int, PagedAndSortedResultRequestDto, CreateUpdateYorumDto>,
        IYorumAppService
    {
        public YorumAppService(IRepository<Yorum, int> repository) : base(repository) { }

        public async Task<List<YorumDto>> GetListByUrunIdAsync(int urunId)
        {
            var queryable = await Repository.WithDetailsAsync(y => y.Urun);
            var yorumlar = await queryable
                .Where(y => y.UrunId == urunId)
                .ToListAsync();

            return ObjectMapper.Map<List<Yorum>, List<YorumDto>>(yorumlar);
        }

        protected override async Task<IQueryable<Yorum>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return (await base.CreateFilteredQueryAsync(input)).Include(y => y.Urun);
        }
    }
}