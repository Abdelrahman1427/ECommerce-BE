using Application.Common.ProductDTOS;
using Application.Features.Products.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using MediatR;

namespace Application.Features.Products.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request.Product);
            await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // reload with category
            var saved = await _unitOfWork.Repository<Product>()
                .GetByIdWithIncludesAsync(product.Id, new System.Linq.Expressions.Expression<Func<Product, object>>[] { p => p.Category! }, cancellationToken);
            return _mapper.Map<ProductDto>(saved ?? product);
        }
    }
}
