using Application.Common.ProductDTOS;
using Application.Features.Products.Queries;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using MediatR;

namespace Application.Features.Products.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetByIdWithIncludesAsync(request.Id,
                    new System.Linq.Expressions.Expression<Func<Product, object>>[] { p => p.Category! },
                    cancellationToken)
                ?? throw new KeyNotFoundException($"Product with id {request.Id} not found.");

            return _mapper.Map<ProductDto>(product);
        }
    }
}
