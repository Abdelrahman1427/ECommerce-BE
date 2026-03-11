using System.Linq.Expressions;
using Application.Common.BranchDTOS;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class BranchService : GenericService<Branch, GetBranchDTO, object, AddBranchDTO, UpdateBranchDTO>, IBranchService
    {
        public BranchService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BranchService> logger) : base(unitOfWork, mapper, logger) { }

        public async Task<List<BranchLookUp>> GetLookup()
        {
            _logger.LogDebug("Getting branch lookup");
            var Branches = await _unitOfWork.Repository<Branch>()
              .GetAllAsync(
                Array.Empty<Expression<Func<Branch, object>>>(),
                CancellationToken.None
              );

            _logger.LogInformation("Retrieved {Count} branches for lookup", Branches.Count);
            return _mapper.Map<List<BranchLookUp>>(Branches);
        }
    }
}