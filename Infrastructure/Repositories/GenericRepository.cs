using System.Data;
using System.Linq.Expressions;
using Domain.Common;
using Domain.IRepository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ECommerceContext _appContext;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger _logger;
        private readonly ICurrentTenantService _tenantService;

        public GenericRepository(ECommerceContext appContext, ILogger logger, ICurrentTenantService tenantService)
        {
            _appContext = appContext;
            _dbSet = _appContext.Set<T>();
            _logger = logger;
            _tenantService = tenantService;
        }

        public IDbConnection QueryConnection => _appContext.Database.GetDbConnection();

        // Add methods remain the same
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellation)
        {
            try
            {
                await _dbSet.AddAsync(entity, cancellation);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in AddAsync<{typeof(T).Name}>");
                throw;
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellation)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities, cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in AddRangeAsync<{typeof(T).Name}>");
                throw;
            }
        }

        // Find methods with tenant filtering
        public virtual IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            try
            {
                var query = _dbSet.Where(expression);
                return ApplyTenantFilter(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Find<{typeof(T).Name}>");
                throw;
            }
        }

        public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> expression, CancellationToken ct)
        {
            try
            {
                var query = _dbSet.Where(expression);
                query = ApplyTenantFilter(query);
                return await query.ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Find<{typeof(T).Name}>");
                throw;
            }
        }
        public async Task<T?> FindLastAsync(Expression<Func<T, bool>> predicate,Expression<Func<T, object>> orderBy,CancellationToken ct)
        {
            return await _dbSet
                .Where(predicate)
                .OrderByDescending(orderBy)
                .FirstOrDefaultAsync(ct);
        }

        // GetAll with tenant filtering
        public async Task<List<T>> GetAllAsync(Expression<Func<T, object>>[] includes, CancellationToken ct)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                query = ApplyTenantFilter(query);
                return await query.ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetAllAsync<{typeof(T).Name}>");
                throw;
            }
        }

        // GetById - tenant check happens after retrieval
        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellation)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id, cancellation);
                return CheckTenantAccess(entity) ? entity : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetByIdAsync<{typeof(T).Name}>");
                throw;
            }
        }

        public virtual async Task<T?> GetByIdWithIncludesAsync(int id, Expression<Func<T, object>>[] includes, CancellationToken cancellation)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellation);
                return CheckTenantAccess(entity) ? entity : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetByIdAsync<{typeof(T).Name}>");
                throw;
            }
        }

        // Remove with tenant validation
        public virtual async Task RemoveAsync(T entity, CancellationToken cancellation)
        {
            try
            {
                if (!CheckTenantAccess(entity))
                    throw new UnauthorizedAccessException("Access to resource denied");

                _dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Remove<{typeof(T).Name}>");
                throw;
            }
        }
        public async Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken ct)
        {
            try
            {
                if (entities.Any(e => !CheckTenantAccess(e)))
                    throw new UnauthorizedAccessException("Access to resource denied");


                _dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Remove<{typeof(T).Name}>");
                throw;
            }
        }

        // Update with tenant validation
        public virtual async Task UpdateAsync(T entity, CancellationToken cancellation)
        {
            try
            {
                if (!CheckTenantAccess(entity))
                    throw new UnauthorizedAccessException("Access to resource denied");

                _dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Update<{typeof(T).Name}>");
                throw;
            }
        }

        public virtual Task UpdateRange(IEnumerable<T> entities, CancellationToken cancellation)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (!CheckTenantAccess(entity))
                        throw new UnauthorizedAccessException("Access to resource denied");
                }

                _dbSet.UpdateRange(entities);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in UpdateRange<{typeof(T).Name}>");
                throw;
            }
        }

        // Queryable methods with tenant filtering
        public IQueryable<T> GetQueryable(bool asNoTracking = false)
        {
            try
            {
                var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
                return ApplyTenantFilter(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in GetQueryable<{typeof(T).Name}>");
                throw;
            }
        }

        public IQueryable<T> GetQueryableWithIncludes(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return ApplyTenantFilter(query);
        }

        // Other methods with tenant filtering
        //public virtual async Task<List<T>> GetElementsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellation)
        //{
        //    var query = _dbSet.Where(expression);
        //    query = ApplyTenantFilter(query);
        //    return await query.ToListAsync(cancellation);
        //}

        //public virtual async Task<T?> GetOnlyElement(Expression<Func<T, bool>> expression, CancellationToken cancellation)
        //{
        //    var query = _dbSet.Where(expression);
        //    query = ApplyTenantFilter(query);
        //    return await query.FirstOrDefaultAsync(expression, cancellation);
        //}
        public virtual async Task<List<T>> GetElementsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellation)
        {
            return await _dbSet.Where(expression).ToListAsync(cancellation);
        }

        public virtual async Task<T?> GetOnlyElement(Expression<Func<T, bool>> expression, CancellationToken cancellation)
        {
            return await _dbSet.FirstOrDefaultAsync(expression, cancellation);
        }
        // Tenant filtering logic
        private IQueryable<T> ApplyTenantFilter(IQueryable<T> query)
        {
            if (typeof(IBranchEntity).IsAssignableFrom(typeof(T)) && _tenantService.BranchId.HasValue)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var branchIdProperty = Expression.Property(parameter, "BranchId");
                var tenantValue = Expression.Constant(_tenantService.BranchId.Value, typeof(int?));
                var condition = Expression.Equal(branchIdProperty, tenantValue);
                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                query = query.Where(lambda);
            }
            return query;
        }

        private bool CheckTenantAccess(T entity)
        {
            if (entity is IBranchEntity branchEntity && _tenantService.BranchId.HasValue)
            {
                return branchEntity.BranchId == _tenantService.BranchId.Value;
            }
            return true; // Allow if not a branch entity or no tenant context
        }
    }
}