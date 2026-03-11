using Application.Common.Pagination;
using Application.Common.ProductDTOS;
using Application.Features.Products.Queries;
using Application.Helper;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Features.Products.Handlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResponse<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Product>().GetQueryableWithIncludes(p => p.Category).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.Name.Contains(request.Search));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "price" => request.SortDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "name" => request.SortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    _ => query
                };
            }

            var paged = await query.ToPagedResponseAsync(request);
            var dto = paged.Items.Select(p => _mapper.Map<ProductDto>(p)).ToList();
            return new PagedResponse<ProductDto>(dto, paged.TotalCount, paged.PageNumber, paged.PageSize);
        }
    }
}
