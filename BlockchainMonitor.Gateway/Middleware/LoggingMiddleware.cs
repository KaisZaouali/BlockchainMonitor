namespace BlockchainMonitor.Gateway.Middleware;

/// <summary>
/// Logging middleware for API Gateway requests.
/// Logs incoming requests, successful responses, and errors with timing information.
/// Provides detailed request/response logging for debugging and monitoring.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            // Log request
            _logger.LogInformation("🚪 Gateway Request: {Method} {Path} from {UserAgent}", 
                requestMethod, requestPath, context.Request.Headers.UserAgent.ToString());

            await _next(context);

            stopwatch.Stop();

            // Log response
            _logger.LogInformation("✅ Gateway Response: {Method} {Path} - {StatusCode} in {ElapsedMs}ms", 
                requestMethod, requestPath, context.Response.StatusCode, stopwatch.Elapsed.TotalMilliseconds);

            // Console output for easy monitoring
            Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {requestMethod} {requestPath} - {context.Response.StatusCode} ({stopwatch.Elapsed.TotalMilliseconds:F2}ms)");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Log error
            _logger.LogError(ex, "❌ Gateway Error: {Method} {Path} in {ElapsedMs}ms", 
                requestMethod, requestPath, stopwatch.Elapsed.TotalMilliseconds);

            throw;
        }
    }
}