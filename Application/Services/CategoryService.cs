using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Common.BranchDTOS;
using Application.Common.CategoryDTOS;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CategoryService : GenericService<Category, CategoryDTO, object, AddCategoryDTO, UpdateCategoryDTO>, ICategoryService
    {
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger) : base(unitOfWork, mapper, logger)
        { }

        protected override Expression<Func<Category, object>>[] includes => new Expression<Func<Category, object>>[]
        {
             c=>c.Products
        };
    }
}
