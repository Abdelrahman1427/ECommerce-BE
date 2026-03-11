using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
          RequestDelegate next,
          ILogger<GlobalExceptionHandlerMiddleware> logger,
          IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case AppException appEx:
                    errorResponse.StatusCode = appEx.StatusCode;
                    errorResponse.Message = appEx.Message;
                    errorResponse.ErrorCode = appEx.ErrorCode;

                    if (appEx is ValidationException validationEx)
                    {
                        errorResponse.Details = validationEx.Errors;
                    }
                    context.Response.StatusCode = appEx.StatusCode;
                    break;

                case KeyNotFoundException:
                    errorResponse.StatusCode = StatusCodes.Status404NotFound;
                    errorResponse.Message = exception.Message;
                    errorResponse.ErrorCode = "NOT_FOUND";
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                case UnauthorizedAccessException:
                    errorResponse.StatusCode = StatusCodes.Status403Forbidden;
                    errorResponse.Message = "Access to this resource is forbidden.";
                    errorResponse.ErrorCode = "FORBIDDEN";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    break;

                case DbUpdateException dbUpdateEx:
                    errorResponse = HandleDatabaseException(dbUpdateEx);
                    context.Response.StatusCode = errorResponse.StatusCode;
                    break;

                case SqlException sqlEx:
                    errorResponse = HandleSqlException(sqlEx);
                    context.Response.StatusCode = errorResponse.StatusCode;
                    break;

                case ArgumentException:
                    errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.Message = exception.Message;
                    errorResponse.ErrorCode = "BAD_REQUEST";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                default:
                    errorResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    errorResponse.Message = _environment.IsDevelopment() ?
                      exception.Message :
                      "An internal server error occurred. Please try again later.";
                    errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    if (_environment.IsDevelopment())
                    {
                        errorResponse.Details = new
                        {
                            exception.StackTrace,
                            InnerException = exception.InnerException?.Message
                        };
                    }
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(result);
        }

        private ErrorResponse HandleDatabaseException(DbUpdateException dbEx)
        {
            var errorResponse = new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorCode = "DATABASE_ERROR"
            };

            if (dbEx.InnerException is SqlException sqlEx)
            {
                return HandleSqlException(sqlEx);
            }

            errorResponse.Message = _environment.IsDevelopment() ? $"Database update failed: {dbEx.Message}" : "Failed to update the database. Please try again.";

            return errorResponse;
        }

        private ErrorResponse HandleSqlException(SqlException sqlEx)
        {
            var errorResponse = new ErrorResponse();

            // Foreign key violation
            if (sqlEx.Message.Contains("REFERENCE constraint") || sqlEx.Number == 547)
            {
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = "Cannot delete or modify this item because it is linked to other data.";
                errorResponse.ErrorCode = "FOREIGN_KEY_VIOLATION";
                return errorResponse;
            }

            // Unique constraint violation
            if (sqlEx.Message.Contains("UNIQUE KEY") || sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                errorResponse.StatusCode = StatusCodes.Status409Conflict;
                errorResponse.Message = "This item already exists. Please use a different value.";
                errorResponse.ErrorCode = "DUPLICATE_ENTRY";
                return errorResponse;
            }

            // Timeout
            if (sqlEx.Number == -2)
            {
                errorResponse.StatusCode = StatusCodes.Status408RequestTimeout;
                errorResponse.Message = "The database operation timed out. Please try again.";
                errorResponse.ErrorCode = "DATABASE_TIMEOUT";
                return errorResponse;
            }

            // Default SQL error
            errorResponse.StatusCode = StatusCodes.Status500InternalServerError;
            errorResponse.Message = _environment.IsDevelopment() ? $"Database error: {sqlEx.Message}" : "A database error occurred. Please try again.";
            errorResponse.ErrorCode = "SQL_ERROR";

            if (_environment.IsDevelopment())
            {
                errorResponse.Details = new
                {
                    SqlErrorNumber = sqlEx.Number,
                    SqlState = sqlEx.State,
                    LineNumber = sqlEx.LineNumber
                };
            }

            return errorResponse;
        }
    }
}