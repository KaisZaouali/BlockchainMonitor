using BlockchainMonitor.Infrastructure.Interfaces;

namespace BlockchainMonitor.Gateway.Middleware;

/// <summary>
/// Middleware for tracking metrics on API Gateway requests.
/// Records request counts, response times, errors, and rate limiting events.
/// </summary>
public class MetricsTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMetricsService _metricsService;

    public MetricsTrackingMiddleware(RequestDelegate next, IMetricsService metricsService)
    {
        _next = next;
        _metricsService = metricsService;
    }

    /// <summary>
    /// Processes the HTTP request and records relevant metrics.
    /// </summary>
    /// <param name="context">The HTTP context for the request</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.HasValue || context.Request.Path.Value.Contains("metrics") || context.Request.Path.Value.Contains("health") || context.Request.Path.Value.Contains("swagger"))
        {
            await _next(context);
            return;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value ?? "";
        var requestMethod = context.Request.Method;

        try
        {
            // Track request
            _metricsService.IncrementRequestCount($"{requestMethod} {requestPath}");
            
            await _next(context);
            
            stopwatch.Stop();
            
            // Track response time
            _metricsService.RecordResponseTime($"{requestMethod} {requestPath}", stopwatch.Elapsed);
            
            // Check if rate limit was exceeded (429 status code)
            if (context.Response.StatusCode == 429)
            {
                // Track rate limit exceeded
                _metricsService.IncrementRateLimitExceeded($"{requestMethod} {requestPath}");
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Track error
            _metricsService.IncrementErrorCount($"{requestMethod} {requestPath}", ex.GetType().Name);
            
            throw;
        }
    }
} 