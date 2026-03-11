namespace Application.Common.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public int StatusCode { get; set; }
        public string? TraceId { get; set; }
        public object? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
