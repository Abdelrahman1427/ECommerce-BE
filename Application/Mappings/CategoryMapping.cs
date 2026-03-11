using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.CategoryDTOS;
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

            CreateMap<Category, CategoryDTO>();
        }
    }
}
