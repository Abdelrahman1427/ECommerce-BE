using Application.Common.ProductDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            // Product → ProductDto: split comma-separated ImageUrls into List<string>
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s =>
                    s.Category != null ? s.Category.Name : string.Empty))
                .ForMember(d => d.Images, o => o.MapFrom(s =>
                    !string.IsNullOrWhiteSpace(s.ImageUrls)
                        ? s.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(u => u.Trim()).Where(u => !string.IsNullOrWhiteSpace(u)).ToList()
                        : new List<string>()));

            // CreateProductDto → Product: join List<string> into comma-separated string
            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
                    s.ImageUrls != null && s.ImageUrls.Any()
                        ? string.Join(',', s.ImageUrls.Select(u => u.Trim()).Where(u => !string.IsNullOrEmpty(u)))
                        : null));

            // UpdateProductDto → Product: ignore ImageUrls here — handler does it manually
            CreateMap<UpdateProductDto, Product>()
                .ForMember(d => d.ImageUrls, o => o.Ignore())
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}