using System.Diagnostics;

namespace GetFlight.API.Middleware
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

            try
            {
                await _next(context);
                stopwatch.Stop();

                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (context.Response.StatusCode < 400) // Success
                {
                    _logger.LogInformation(
                        "Request {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        elapsedMilliseconds);
                }
                else // Error
                {
                    _logger.LogWarning(
                        "Request {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        elapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Request {Method} {Path} failed with {Exception} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
