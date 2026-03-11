# WayFinder Backend - Final Touches Documentation

This document describes the final improvements made to the WayFinder backend before the first release.

## Overview of Improvements

1. ? **Pagination with Filtering and Sorting**
2. ? **Global Exception Handling Middleware**
3. ? **Comprehensive Logging with Serilog**
4. ? **Refactored Repository Pattern**

---

## 1. Pagination with Filtering and Sorting

### Features Added

- **Basic Pagination**: Existing `PagingDTO` for simple page-based results
- **Advanced Filtering**: New `FilteredPagingDTO` with search, sort, and direction support
- **Generic Implementation**: Works across all entities automatically

### Usage

#### Basic Pagination
```http
GET /api/Vertex/paged?PageNumber=1&PageSize=10
```

Response:
```json
{
  "items": [...],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

#### Advanced Filtering
```http
GET /api/Vertex/filtered?PageNumber=1&PageSize=10&SearchTerm=room&SortBy=EnName&SortDescending=false
```

### Implementation Details

**DTOs:**
- `PagingDTO`: Basic pagination (PageNumber, PageSize)
- `FilteredPagingDTO`: Advanced (adds SearchTerm, SortBy, SortDescending)

**Service Methods:**
- `GetPagedAsync()`: Basic pagination
- `GetFilteredPagedAsync()`: With filtering and sorting

**Customization:**
Override `ApplySearchFilter()` in derived services for entity-specific search logic:

```csharp
protected override IQueryable<Vertex> ApplySearchFilter(IQueryable<Vertex> query, string searchTerm)
{
    return query.Where(v => 
   v.EnName.Contains(searchTerm) || 
        v.ArName.Contains(searchTerm));
}
```

---

## 2. Global Exception Handling Middleware

### Features

- **Centralized Error Handling**: All exceptions caught and formatted consistently
- **Environment-Aware**: Detailed errors in Development, user-friendly in Production
- **Custom Exception Types**: Specific exceptions for common scenarios
- **SQL Error Handling**: Translates database errors to user-friendly messages

### Custom Exception Types

```csharp
// Application/Common/Exceptions/AppExceptions.cs

NotFoundException // 404
ValidationException // 400 with validation details
UnauthorizedException      // 401
ForbiddenException         // 403
ConflictException         // 409
AppException              // Base class for custom exceptions
```

### Usage Example

```csharp
public async Task<VertexDetailsDTO?> GetVertex(string id)
{
    var vertex = await _repository.FindAsync(id);
    if (vertex == null)
    {
        throw new NotFoundException("Vertex", id);
 }
    return vertex;
}
```

### Error Response Format

```json
{
  "success": false,
  "message": "Vertex with identifier 'V001' was not found.",
  "errorCode": "NOT_FOUND",
  "statusCode": 404,
  "traceId": "0HMVID1234567",
  "timestamp": "2024-01-15T10:30:00Z",
  "details": null  // Only in Development
}
```

### Handled Exceptions

| Exception Type | Status Code | Error Code | Message |
|---|---|---|---|
| `NotFoundException` | 404 | NOT_FOUND | Entity not found |
| `ValidationException` | 400 | VALIDATION_ERROR | Validation errors |
| `UnauthorizedException` | 401 | UNAUTHORIZED | Unauthorized access |
| `ForbiddenException` | 403 | FORBIDDEN | Access forbidden |
| `ConflictException` | 409 | CONFLICT | Resource conflict |
| `KeyNotFoundException` | 404 | NOT_FOUND | Resource not found |
| `UnauthorizedAccessException` | 403 | FORBIDDEN | Access denied |
| `DbUpdateException` | 400/500 | DATABASE_ERROR | Database operation failed |
| SQL Foreign Key | 400 | FOREIGN_KEY_VIOLATION | Linked data exists |
| SQL Unique Constraint | 409 | DUPLICATE_ENTRY | Duplicate value |
| SQL Timeout | 408 | DATABASE_TIMEOUT | Operation timed out |
| `ArgumentException` | 400 | BAD_REQUEST | Invalid argument |
| Others | 500 | INTERNAL_SERVER_ERROR | Unexpected error |

---

## 3. Comprehensive Logging with Serilog

### Features

- **Structured Logging**: JSON-based log format
- **Multiple Sinks**: Console and File logging
- **Log Levels**: Configurable per namespace
- **Request Logging**: Automatic HTTP request/response logging
- **Performance Tracking**: Request duration tracking
- **Log Retention**: 30-day file rotation

### Configuration

Located in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
  "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Warning"
   }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
  {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
     "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

### Log Levels

- **Debug**: Detailed diagnostic information
- **Information**: General application flow
- **Warning**: Potential issues or unexpected behavior
- **Error**: Errors and exceptions
- **Fatal**: Critical failures

### Logging in Services

All services now have comprehensive logging:

```csharp
_logger.LogInformation("Creating new {EntityName}", typeof(TEntity).Name);
_logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
_logger.LogError(ex, "Error in {Method}", nameof(CreateAsync));
```

### Request Logging

Every HTTP request is automatically logged with:
- HTTP Method
- Path
- User (if authenticated)
- Status Code
- Duration (in milliseconds)
- TraceId (for correlation)

Example log output:
```
[10:30:15 INF] HTTP GET /api/Vertex/1 started. TraceId: 0HMVID1234567, User: admin@example.com
[10:30:15 INF] HTTP GET /api/Vertex/1 completed with 200 in 45ms. TraceId: 0HMVID1234567
```

### Log File Location

Logs are stored in `Logs/` directory:
- File pattern: `log-YYYYMMDD.txt`
- New file created daily
- Last 30 files retained
- Older files automatically deleted

---

## 4. Repository Pattern Refactoring

### Improvements Made

- **Consistent Error Handling**: All repository methods properly handle exceptions
- **Logging Integration**: Comprehensive logging in repository operations
- **Tenant Filtering**: Automatic multi-tenancy support via `IBranchEntity`
- **Generic Operations**: CRUD operations available for all entities
- **Query Flexibility**: Support for includes, filtering, and raw queries

### Key Repository Methods

```csharp
// Basic CRUD
Task<T> AddAsync(T entity, CancellationToken ct);
Task<T?> GetByIdAsync(int id, CancellationToken ct);
Task UpdateAsync(T entity, CancellationToken ct);
Task RemoveAsync(T entity, CancellationToken ct);

// With Includes
Task<T?> GetByIdWithIncludesAsync(int id, Expression<Func<T, object>>[] includes, CancellationToken ct);
Task<List<T>> GetAllAsync(Expression<Func<T, object>>[] includes, CancellationToken ct);

// Querying
IQueryable<T> GetQueryable(bool asNoTracking = false);
IQueryable<T> GetQueryableWithIncludes(params Expression<Func<T, object>>[] includes);
Task<List<T>> FindAsync(Expression<Func<T, bool>> expression, CancellationToken ct);
Task<T?> GetOnlyElement(Expression<Func<T, bool>> expression, CancellationToken ct);
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

Usage:
```csharp
var vertex = await _unitOfWork.Repository<Vertex>().GetByIdAsync(id, ct);
await _unitOfWork.SaveChangesAsync(ct);
```

### Multi-Tenancy Support

Entities implementing `IBranchEntity` automatically filter by `BranchId`:

```csharp
public class Vertex : BranchBaseAuditableEntity
{
    // Automatically filtered by current tenant's BranchId
}
```

---

## Middleware Registration

The middleware is registered in `Program.cs` in the correct order:

```csharp
// 1. Exception Handler (must be first)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// 2. Request Logging
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Standard ASP.NET Core middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
```

---

## Testing the Improvements

### 1. Test Pagination

```bash
# Basic pagination
curl "https://localhost:7001/api/Vertex/paged?PageNumber=1&PageSize=5"

# Advanced filtering
curl "https://localhost:7001/api/Vertex/filtered?PageNumber=1&PageSize=5&SearchTerm=room&SortBy=Id&SortDescending=false"
```

### 2. Test Exception Handling

```bash
# Not found (404)
curl "https://localhost:7001/api/Vertex/99999"

# Validation error (400)
curl -X POST "https://localhost:7001/api/Vertex" \
  -H "Content-Type: application/json" \
  -d "{}"
```

### 3. Check Logs

- **Console**: View real-time logs in the terminal
- **File**: Check `Logs/log-YYYYMMDD.txt` for detailed logs

---

## Performance Considerations

1. **Pagination**: Reduces memory usage by loading only requested page
2. **Logging**: Async file writing prevents blocking
3. **Exception Handling**: Minimal performance impact
4. **Repository**: Efficient query execution with optional tracking

---

## Security Improvements

1. **Error Messages**: Sensitive information hidden in Production
2. **Logging**: PII can be excluded from logs
3. **Tenant Isolation**: Automatic branch-level data isolation
4. **Trace IDs**: Easy correlation without exposing internals

---

## Migration Notes

### Breaking Changes
- **Services**: All services now require `ILogger` parameter
- **GenericService**: Constructor signature changed

### Migration Steps

1. Update service constructors to include logger:
```csharp
// Old
public MyService(IUnitOfWork unitOfWork, IMapper mapper) 
    : base(unitOfWork, mapper)

// New
public MyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MyService> logger) 
    : base(unitOfWork, mapper, logger)
```

2. Packages added:
   - `Serilog.AspNetCore` (9.0.0)
 - `Serilog.Sinks.File` (7.0.0)

---

## Future Enhancements

1. **Logging**: Add structured log enrichers (machine name, environment)
2. **Monitoring**: Integrate Application Insights or Seq
3. **Caching**: Add Redis caching for frequent queries
4. **Rate Limiting**: Implement API rate limiting
5. **Health Checks**: Add health check endpoints

---

## Support

For issues or questions, please refer to:
- Application logs in `Logs/` directory
- TraceId in error responses for correlation
- Console output during development

---

**Version**: 1.0.0  
**Last Updated**: 2024-01-15  
**Author**: WayFinder Development Team
