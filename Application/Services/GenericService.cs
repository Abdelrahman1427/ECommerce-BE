using Application.Common.Pagination;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;


namespace Application.Services
{
    public class GenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO> : IGenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO>
    where TEntity : class
    where TGetDTO : class
    where TFilter : class?
    where TAddDTO : class
    where TUpdateDTO : class
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly ILogger<GenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO>> _logger;
        protected virtual Expression<Func<TEntity, object>>[] includes => Array.Empty<Expression<Func<TEntity, object>>>();

        public GenericService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO>> logger)
        {
            _unitOfWork = unitOfWork;
             _repository = _unitOfWork.Repository<TEntity>();
            _mapper = mapper;
            _logger = logger;

        }

        public virtual async Task<List<TGetDTO>> GetAllAsync(CancellationToken ct)
        {
            _logger.LogDebug("Getting all {EntityName}", typeof(TEntity).Name);
            var entity = await _repository.GetAllAsync(includes, ct);
            _logger.LogInformation("Retrieved {Count} {EntityName} records", entity.Count, typeof(TEntity).Name);
            return _mapper.Map<List<TGetDTO>>(entity);
        }

        public virtual async Task<TGetDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            _logger.LogDebug("Getting {EntityName} by ID: {Id}", typeof(TEntity).Name, id);
            var entity = await _repository.GetByIdAsync(id, ct);

            if (entity == null)
            {
                _logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                return null;
            }

            return _mapper.Map<TGetDTO>(entity);
        }

        public virtual async Task<TGetDTO?> GetByIdWithIncludesAsync(int id, CancellationToken ct)
        {
            _logger.LogDebug("Getting {EntityName} by ID with includes: {Id}", typeof(TEntity).Name, id);
            var entity = await _repository.GetByIdWithIncludesAsync(id, includes, ct);

            if (entity == null)
            {
                _logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
            }

            return entity == null ? null : _mapper.Map<TGetDTO>(entity);
        }

        public virtual async Task<TGetDTO?> CreateAsync(TAddDTO dto, CancellationToken ct)
        {
            _logger.LogInformation("Creating new {EntityName}", typeof(TEntity).Name);
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Successfully created {EntityName}", typeof(TEntity).Name);
            return entity == null ? null : _mapper.Map<TGetDTO>(entity);
        }

        public virtual async Task<TGetDTO?> UpdateAsync(int id, TUpdateDTO dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating {EntityName} with ID: {Id}", typeof(TEntity).Name, id);

            var repository = _unitOfWork.Repository<TEntity>();
            
            var entity = await repository.GetByIdAsync(id, ct);

            if (entity == null)
            {
                _logger.LogWarning("Cannot update. {EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with Id={id} not found.");
            }

            _mapper.Map(dto, entity);
            // await repository.UpdateAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            
            _logger.LogInformation("Successfully updated {EntityName} with ID: {Id}", typeof(TEntity).Name, id);

            return _mapper.Map<TGetDTO>(entity);
        }

        public virtual async Task DeleteAsync(int id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting {EntityName} with ID: {Id}", typeof(TEntity).Name, id);
            var repo = _unitOfWork.Repository<TEntity>();
            var entity = await repo.GetByIdAsync(id, ct);

            if (entity == null)
            {
                _logger.LogWarning("Cannot delete. {EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                throw new KeyNotFoundException("Entity with id={id} not found.");
            }

            await repo.RemoveAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Successfully deleted {EntityName} with ID: {Id}", typeof(TEntity).Name, id);
        }

        public virtual async Task<PagedResponse<TGetDTO>> GetFilteredPagedAsync(PagingDTO<TFilter>? paging, CancellationToken ct)
        {
            _logger.LogDebug("Getting filtered paged {EntityName}. Page: {Page}, Size: {Size}, Search: {Search}", typeof(TEntity).Name, paging?.PageNumber, paging?.PageSize, paging?.Filter);
            var query = _unitOfWork.Repository<TEntity>().GetQueryableWithIncludes(includes);

            // Apply search filter if provided
            if (paging?.Filter != null)
            {
                query = ApplyDynamicFilter(query, paging.Filter);
            }


            var projected = query.ProjectTo<TGetDTO>(_mapper.ConfigurationProvider);

            // Apply sorting if provided
            //if (!string.IsNullOrWhiteSpace(paging.SortBy))    
            //{
            //    projected = ApplySorting(projected, paging.SortBy, paging.SortDescending);
            //}

            return await projected.ToPagedResponseAsync(paging ?? new PagingDTO<TFilter>());
        }

        public virtual IQueryable<TEntity> ApplyDynamicFilter(
            IQueryable<TEntity> query,
            TFilter filter)
        {
            _logger.LogDebug("No dynamic filter applied for {Entity}", typeof(TEntity).Name);
            return query;
        }


        protected virtual IQueryable<TGetDTO> ApplySorting(IQueryable<TGetDTO> query, string sortBy, bool descending)
        {
            // Default implementation using reflection
            var property = typeof(TGetDTO).GetProperty(sortBy);
            if (property == null)
            {
                _logger.LogWarning("Sort property {SortBy} not found on {DTOName}", sortBy, typeof(TGetDTO).Name);
                return query;
            }

            var parameter = Expression.Parameter(typeof(TGetDTO), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
            typeof(Queryable), methodName, new Type[] {
        typeof(TGetDTO),
        property.PropertyType
            },
            query.Expression, Expression.Quote(lambda));

            return query.Provider.CreateQuery<TGetDTO>(resultExpression);
        }
    }
}