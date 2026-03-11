using Application.Common.ProductDTOS;
using MediatR;

namespace Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public int Id { get; set; }
        public UpdateProductDto Product { get; set; } = new();
    }
}
