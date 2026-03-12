using Application.Common.CategoryDTOS;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<AddCategoryDTO, Category>();

            // Fix: map UpdateCategoryDTO -> Category (only non-null fields)
            CreateMap<UpdateCategoryDTO, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Category, CategoryDTO>();
        }
    }
}