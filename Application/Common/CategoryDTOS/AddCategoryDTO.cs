using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ProductDTOS;
using Domain.Entities;

namespace Application.Common.CategoryDTOS
{
    public class AddCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        //public List<ProductDto> Products { get; set; } = new();
    }
}
