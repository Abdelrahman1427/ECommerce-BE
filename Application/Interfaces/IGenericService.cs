using System.Linq.Expressions;
using Application.Common.Pagination;

namespace Application.Interfaces
{
    public interface IGenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO>
        where TEntity : class
        where TGetDTO : class
        where TFilter : class?
        where TAddDTO : class
        where TUpdateDTO : class
    {
        Task<List<TGetDTO>> GetAllAsync( CancellationToken ct);
        Task<PagedResponse<TGetDTO>> GetFilteredPagedAsync(PagingDTO<TFilter>? paging, CancellationToken ct);
        Task<TGetDTO?> GetByIdAsync(int id, CancellationToken ct);
        Task<TGetDTO?> CreateAsync(TAddDTO dto, CancellationToken ct);
        Task<TGetDTO?> UpdateAsync(int id, TUpdateDTO dto, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
        IQueryable<TEntity> ApplyDynamicFilter(IQueryable<TEntity> query, TFilter filter);
    }

}
