using System.Diagnostics;

namespace BlockchainMonitor.Gateway.Middleware;

public class GatewayMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayMiddleware> _logger;

    public GatewayMiddleware(RequestDelegate next, ILogger<GatewayMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalPath = context.Request.Path;
        var originalMethod = context.Request.Method;

        try
        {
            _logger.LogInformation("Gateway request: {Method} {Path}", originalMethod, originalPath);
            
            await _next(context);
            
            stopwatch.Stop();
            _logger.LogInformation("Gateway response: {StatusCode} in {ElapsedMs}ms", 
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Gateway error: {Method} {Path} in {ElapsedMs}ms", 
                originalMethod, originalPath, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

public static class GatewayMiddlewareExtensions
{
    public static IApplicationBuilder UseGatewayMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GatewayMiddleware>();
    }
} 