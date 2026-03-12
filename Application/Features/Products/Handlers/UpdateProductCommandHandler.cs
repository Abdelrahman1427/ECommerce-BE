using Application.Common.ProductDTOS;
using Application.Features.Products.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using MediatR;

namespace Application.Features.Products.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Product>();
            var existing = await repo.GetByIdWithIncludesAsync(
                request.Id,
                new System.Linq.Expressions.Expression<Func<Product, object>>[] { p => p.Category! },
                cancellationToken)
                ?? throw new KeyNotFoundException($"Product with id {request.Id} not found.");

            // Partial update — only apply non-null fields
            if (request.Product.Name != null)
                existing.Name = request.Product.Name;
            if (request.Product.Description != null)
                existing.Description = request.Product.Description;
            if (request.Product.Price.HasValue)
                existing.Price = request.Product.Price.Value;
            if (request.Product.StockQuantity.HasValue)
                existing.StockQuantity = request.Product.StockQuantity.Value;
            if (request.Product.CategoryId.HasValue)
                existing.CategoryId = request.Product.CategoryId.Value;
            if (request.Product.IsActive.HasValue)
                existing.IsActive = request.Product.IsActive.Value;

            // Handle ImageUrls: join list into comma-separated string, trimming blanks
            if (request.Product.ImageUrls != null)
            {
                var cleaned = request.Product.ImageUrls
                    .Select(u => u?.Trim())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList();
                existing.ImageUrls = cleaned.Any() ? string.Join(',', cleaned) : null;
            }

            await repo.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ProductDto>(existing);
        }
    }
}