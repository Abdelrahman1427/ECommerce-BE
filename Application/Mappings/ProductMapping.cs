using Application.Common.ProductDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
                .ForMember(d => d.Images, o => o.MapFrom(s =>
                    s.ImageUrls != null ? s.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : null));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
                    s.ImageUrls != null ? string.Join(',', s.ImageUrls) : null));

            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
