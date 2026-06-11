using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FitAI.Domain.Commerce;
using FitAI.Magazalar;

namespace FitAI.Magazalar
{
    public class MagazaAppService :
        CrudAppService<Magaza, MagazaDto, int, PagedAndSortedResultRequestDto, CreateUpdateMagazaDto>,
        IMagazaAppService
    {
        public MagazaAppService(IRepository<Magaza, int> repository)
            : base(repository)
        {
        }

        public async Task<MagazaDto> GetByEpostaAsync(string eposta)
        {
            var magaza = await Repository.FirstOrDefaultAsync(m => m.Eposta == eposta);
            return ObjectMapper.Map<Magaza, MagazaDto>(magaza);
        }

        protected override async Task<IQueryable<Magaza>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            return query.Where(m => m.AktifMi);
        }

        public override async Task DeleteAsync(int id)
        {
            var magaza = await Repository.GetAsync(id);
            magaza.AktifMi = false;
            await Repository.UpdateAsync(magaza);
        }
    }
}