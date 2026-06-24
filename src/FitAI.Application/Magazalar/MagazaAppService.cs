using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Entities;
using Volo.Abp;
using FitAI.Domain.Commerce;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace FitAI.Magazalar
{
    public class MagazaAppService :
        CrudAppService<Magaza, MagazaDto, int, PagedAndSortedResultRequestDto, CreateUpdateMagazaDto>,
        IMagazaAppService
    {
        private readonly IRepository<Magaza, int> _repository;

        public MagazaAppService(IRepository<Magaza, int> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public async Task<MagazaDto> GetByEpostaAsync(string eposta)
        {
            if (string.IsNullOrWhiteSpace(eposta))
                throw new UserFriendlyException("E-posta adresi gereklidir.");

            var magaza = await _repository.FirstOrDefaultAsync(m => m.Eposta == eposta);
            if (magaza == null)
                throw new UserFriendlyException($"E-posta '{eposta}' ile mağaza bulunamadı.");

            return ObjectMapper.Map<Magaza, MagazaDto>(magaza);
        }

        public override async Task<MagazaDto> CreateAsync(CreateUpdateMagazaDto input)
        {
            // Tüm validasyonlar burada, try-catch yok, direkt UserFriendlyException fırlat
            if (input == null)
                throw new UserFriendlyException("Mağaza bilgileri gereklidir.");

            if (string.IsNullOrWhiteSpace(input.MagazaAdi))
                throw new UserFriendlyException("Mağaza adı gereklidir.");

            if (string.IsNullOrWhiteSpace(input.Eposta))
                throw new UserFriendlyException("E-posta adresi gereklidir.");

            if (!input.Eposta.Contains("@"))
                throw new UserFriendlyException("Geçerli bir e-posta adresi girin.");

            if (string.IsNullOrWhiteSpace(input.SifreHash) || input.SifreHash.Length < 6)
                throw new UserFriendlyException("Şifre en az 6 karakter olmalıdır.");

            if (string.IsNullOrWhiteSpace(input.PaketTuru))
                throw new UserFriendlyException("Paket türü gereklidir.");

            var existingMagaza = await _repository.FirstOrDefaultAsync(m => m.Eposta == input.Eposta);
            if (existingMagaza != null)
                throw new UserFriendlyException($"E-posta '{input.Eposta}' zaten kayıtlı.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.SifreHash);

            var entity = new Magaza
            {
                MagazaAdi = input.MagazaAdi.Trim(),
                Eposta = input.Eposta.Trim().ToLower(),
                SifreHash = hashedPassword,
                KomisyonOrani = input.KomisyonOrani,
                MinimumKomisyonEsigi = input.MinimumKomisyonEsigi ?? 0m,
                PaketTuru = input.PaketTuru.Trim(),
                AktifMi = input.AktifMi
            };

            var createdEntity = await _repository.InsertAsync(entity);
            // ABP UOW otomatik kaydeder

            Logger.LogInformation($"Mağaza oluşturuldu: ID={createdEntity.Id}");
            return ObjectMapper.Map<Magaza, MagazaDto>(createdEntity);
        }

        public override async Task<MagazaDto> UpdateAsync(int id, CreateUpdateMagazaDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Mağaza bilgileri gereklidir.");

            if (string.IsNullOrWhiteSpace(input.MagazaAdi))
                throw new UserFriendlyException("Mağaza adı gereklidir.");

            if (string.IsNullOrWhiteSpace(input.Eposta))
                throw new UserFriendlyException("E-posta adresi gereklidir.");

            var entity = await _repository.GetAsync(id);

            if (entity.Eposta != input.Eposta)
            {
                var existing = await _repository.FirstOrDefaultAsync(m => m.Eposta == input.Eposta);
                if (existing != null)
                    throw new UserFriendlyException($"E-posta '{input.Eposta}' zaten kullanılıyor.");
            }

            entity.MagazaAdi = input.MagazaAdi.Trim();
            entity.Eposta = input.Eposta.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(input.SifreHash) && input.SifreHash != "degismedi")
            {
                if (input.SifreHash.Length < 6)
                    throw new UserFriendlyException("Şifre en az 6 karakter olmalıdır.");
                entity.SifreHash = BCrypt.Net.BCrypt.HashPassword(input.SifreHash);
            }

            entity.KomisyonOrani = input.KomisyonOrani;
            entity.MinimumKomisyonEsigi = input.MinimumKomisyonEsigi ?? 0m;
            entity.PaketTuru = input.PaketTuru.Trim();
            entity.AktifMi = input.AktifMi;

            await _repository.UpdateAsync(entity);
            Logger.LogInformation($"Mağaza güncellendi: ID={id}");
            return ObjectMapper.Map<Magaza, MagazaDto>(entity);
        }

        protected override async Task<IQueryable<Magaza>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await base.CreateFilteredQueryAsync(input);
            return query.Where(m => m.AktifMi);
        }

        public override async Task DeleteAsync(int id)
        {
            var magaza = await _repository.GetAsync(id);
            magaza.AktifMi = false;
            await _repository.UpdateAsync(magaza);
            Logger.LogInformation($"Mağaza silindi (soft): ID={id}");
        }
    }
}