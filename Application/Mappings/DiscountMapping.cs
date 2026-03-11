using Application.Common.DiscountDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class DiscountMapping : Profile
    {
        public DiscountMapping()
        {
            CreateMap<Discount, DiscountDto>()
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));

            CreateMap<AddDiscountDTO, Discount>()
                .ForMember(d => d.Code, o => o.MapFrom(s => s.Code.ToUpper()));
        }
    }
}
