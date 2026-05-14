using AutoMapper;
using FitAI.Domain.Commerce;
using FitAI.Domain.Products;
using FitAI.Magazalar;
using FitAI.Urunler;
using FitAI.Yorumlar;

namespace FitAI;

public class FitAIApplicationAutoMapperProfile : Profile
{
    public FitAIApplicationAutoMapperProfile()
    {
        CreateMap<Urun, UrunDto>()
            .ForMember(dest => dest.MagazaAdi, opt => opt.MapFrom(src => src.Magaza.MagazaAdi));

        CreateMap<CreateUpdateUrunDto, Urun>();

        CreateMap<Yorum, YorumDto>()
    .ForMember(dest => dest.UrunAdi, opt => opt.MapFrom(src => src.Urun.Ad));
        CreateMap<CreateUpdateYorumDto, Yorum>();

        CreateMap<Magaza, MagazaDto>();
        CreateMap<CreateUpdateMagazaDto, Magaza>();
    }
}