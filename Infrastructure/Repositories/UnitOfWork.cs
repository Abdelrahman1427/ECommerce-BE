using Domain.Common;
using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ECommerceContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly ICurrentTenantService _currentTenantService;

    public ECommerceContext Context => _context;

    public UnitOfWork(ECommerceContext context, ILoggerFactory loggerFactory, ICurrentTenantService currentTenantService)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        _currentTenantService = currentTenantService;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories.TryGetValue(typeof(T), out var repo))
            return (IGenericRepository<T>)repo;

        var newRepo = new GenericRepository<T>(_context, _loggerFactory.CreateLogger<GenericRepository<T>>(), _currentTenantService);
        _repositories[typeof(T)] = newRepo;
        return newRepo;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            //var branchId = _currentTenantService.BranchId;
            var userName = _currentTenantService.UserName;
            //var now = DateTimeOffset.UtcNow;

            //var nowWithoutSeconds = new DateTimeOffset(now.Year, now.Month,now.Day,now.Hour,now.Minute, now.Second, 0, now.Offset);


            //foreach (var entry in _context.ChangeTracker.Entries()
            //             .Where(e => e.State == EntityState.Added && e.Entity is IBranchEntity))
            //{
            //    var entity = (IBranchEntity)entry.Entity;

            //    entity.BranchId ??= branchId;

            //}
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                //if (entry.State == EntityState.Added && entry.Entity is IBranchEntity branchEntity)
                //{
                //    branchEntity.BranchId ??= branchId;
                //}

                if (entry.Entity is BaseAuditableEntity auditable)
                {
                    var now = DateTimeOffset.UtcNow;
                    var nowWithoutMilliseconds = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Offset);
                    if (entry.State == EntityState.Added)
                    {
                        auditable.Created = nowWithoutMilliseconds;
                        auditable.CreatedBy = userName;
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        auditable.LastModified = nowWithoutMilliseconds;
                        auditable.LastModifiedBy = userName;
                    }
                }
            }
            var result = await _context.SaveChangesAsync(ct);
            _context.ChangeTracker.Clear();
            return result;
        }

        catch (Exception ex)
        {
            throw new InvalidOperationException("Error saving changes to the database.", ex);
        }
    }

    public void Dispose() => _context.Dispose();
}
