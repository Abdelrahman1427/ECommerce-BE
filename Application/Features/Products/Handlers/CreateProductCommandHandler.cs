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
            // Clean image URLs before saving
            if (request.Product.ImageUrls != null)
            {
                request.Product.ImageUrls = request.Product.ImageUrls
                    .Select(u => u?.Trim())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList();
            }

            var product = _mapper.Map<Product>(request.Product);
            await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with Category include so CategoryName & Images are populated
            var saved = await _unitOfWork.Repository<Product>()
                .GetByIdWithIncludesAsync(
                    product.Id,
                    new System.Linq.Expressions.Expression<Func<Product, object>>[] { p => p.Category! },
                    cancellationToken);

            return _mapper.Map<ProductDto>(saved ?? product);
        }
    }
}