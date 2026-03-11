using Application.Common.Pagination;
using Application.Common.ProductDTOS;
using MediatR;

namespace Application.Features.Products.Queries
{
    public class GetProductsQuery : PagingDTO, IRequest<PagedResponse<ProductDto>>
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; }
    }
}
