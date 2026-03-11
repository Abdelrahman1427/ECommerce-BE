using System.Diagnostics;

namespace API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = context.TraceIdentifier;

            // Log request
            _logger.LogInformation(
                 "HTTP {Method} {Path} started. TraceId: {TraceId}, User: {User}",
                    context.Request.Method,
                   context.Request.Path,
                        requestId,
                  context.User?.Identity?.Name ?? "Anonymous"
               );

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Log response
                _logger.LogInformation(
               "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms. TraceId: {TraceId}",
              context.Request.Method,
                 context.Request.Path,
                  context.Response.StatusCode,
             stopwatch.ElapsedMilliseconds,
                    requestId
                 );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                            ex,
                 "HTTP {Method} {Path} failed with exception after {ElapsedMilliseconds}ms. TraceId: {TraceId}",
                     context.Request.Method,
             context.Request.Path,
                  stopwatch.ElapsedMilliseconds,
           requestId
                   );
                throw;
            }
        }
    }
}
