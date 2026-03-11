using Application.Features.Products.Commands;
using Domain.Entities;
using Domain.IRepository;
using MediatR;

namespace Application.Features.Products.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Product>();
            var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Product with id {request.Id} not found.");

            await repository.RemoveAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
