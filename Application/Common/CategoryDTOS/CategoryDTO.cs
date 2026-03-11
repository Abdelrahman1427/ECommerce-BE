using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ProductDTOS;

namespace Application.Common.CategoryDTOS
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ProductDto> Products { get; set; } = new();
    }
}
