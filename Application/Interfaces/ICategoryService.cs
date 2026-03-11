using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.BranchDTOS;
using Application.Common.CategoryDTOS;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICategoryService : IGenericService<Category , CategoryDTO, object, AddCategoryDTO, UpdateCategoryDTO>
    {
    }
}
