using AutoMapper;
using FitAI.Domain.Commerce;

namespace FitAI.Magazalar
{
    public class MagazaProfile : Profile
    {
        public MagazaProfile()
        {
            CreateMap<Magaza, MagazaDto>();
            CreateMap<CreateUpdateMagazaDto, Magaza>()
                .ForMember(dest => dest.SifreHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}