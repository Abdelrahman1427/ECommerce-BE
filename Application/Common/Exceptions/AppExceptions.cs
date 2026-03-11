namespace Application.Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string? ErrorCode { get; }

        public AppException(string message, int statusCode = 400, string? errorCode = null)
     : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
                 : base(message, 404, "NOT_FOUND")
        {
        }

        public NotFoundException(string entityName, object key)
   : base($"{entityName} with identifier '{key}' was not found.", 404, "NOT_FOUND")
        {
        }
    }

    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", 400, "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized access")
       : base(message, 401, "UNAUTHORIZED")
        {
        }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Access forbidden")
   : base(message, 403, "FORBIDDEN")
        {
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message)
     : base(message, 409, "CONFLICT")
        {
        }
    }
}
