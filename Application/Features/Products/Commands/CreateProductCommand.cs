using Application.Common.ProductDTOS;
using MediatR;

namespace Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public CreateProductDto Product { get; set; } = new();
    }
}
