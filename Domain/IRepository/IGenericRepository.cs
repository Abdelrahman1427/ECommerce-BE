using System.Data;
using System.Linq.Expressions;

namespace Domain.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, object>>[] includes, CancellationToken ct);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellation);
        Task<T?> GetByIdWithIncludesAsync(int id, Expression<Func<T, object>>[] includes, CancellationToken cancellation);

        Task<T> AddAsync(T entity, CancellationToken cancellation);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellation);
        Task UpdateAsync(T entity, CancellationToken cancellation);
        Task UpdateRange(IEnumerable<T> entities, CancellationToken cancellation);
        Task RemoveAsync(T entity, CancellationToken cancellation);
        Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken ct);

        Task<T?> FindLastAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, CancellationToken ct);
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> expression, CancellationToken cancellation);

        IDbConnection QueryConnection { get; }
        IQueryable<T> GetQueryable(bool asNoTracking = false);
        IQueryable<T> GetQueryableWithIncludes(params Expression<Func<T, object>>[] includes);

        //Get array of elements related to one element (ex: all vertices per one floor)
        //Task<List<T>> GetElementsAsync(Expression<Func<T, bool>> expression, CancellationToken cancellation);
        //Get only one element based on a condition (ex: get ScreenId for a specific MacAddress)
        Task<T?> GetOnlyElement(Expression<Func<T, bool>> expression, CancellationToken cancellation);
        //Task GetAllAsync();
    }
}