//using System;
//using Domain.Entities;
//using Domain.IRepository;
//using Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace Infrastructure.Repositories;

//internal class BranchRepository : GenericRepository<Branch>, IBranchRepository
//{
//    private readonly DbSet<Branch> _dbSet;
//    private readonly ECommerceContext _appContext;
//    private readonly ILogger _logger;

//    public BranchRepository(ECommerceContext context, ILogger logger) : base(context, logger) {

//        _appContext = context;
//        _dbSet = _appContext.Set<Branch>();
//        _logger = logger;

//    }

//    public async Task UpdateAsync(Branch branch, CancellationToken ct)
//        {
//            await base.UpdateAsync(branch, ct);
//        }
//        public override async Task AddRangeAsync(IEnumerable<Branch> entities, CancellationToken cancellation)
//        {
//            try
//            {
//                await AddRangeAsync(entities, cancellation);
//                   // OR  Add Directly to _dbSet
//                  await _dbSet.AddRangeAsync(entities, cancellation);
//            }
//            catch (Exception ex)
//            {
//            _logger.LogError(ex, $"Error occurred in AddRangeAsync<{typeof(T).Name}>");
//                throw;
//            }
//        }


//}
