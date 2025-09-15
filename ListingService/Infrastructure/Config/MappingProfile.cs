using Application.DTOs.Responses;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Config;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Listing, ListingDto>()
            .ForMember(d => d.ThumbnailUrl, opt => opt.MapFrom(src => src.PhotoUrls.FirstOrDefault()));

        CreateMap<Listing, ListingDetailDto>()
            .ForMember(d => d.PriceAmount, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(d => d.PriceCurrency, opt => opt.MapFrom(src => src.Price.Currency));
    }
}
