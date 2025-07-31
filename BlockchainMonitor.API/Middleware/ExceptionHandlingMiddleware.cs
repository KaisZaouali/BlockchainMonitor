using System.Net;
using System.Text.Json;
using BlockchainMonitor.Application.Exceptions;

namespace BlockchainMonitor.API.Middleware;

/// <summary>
/// Global exception handling middleware for the API.
/// Catches and processes all unhandled exceptions, providing consistent error responses.
/// Logs exceptions and returns appropriate HTTP status codes based on exception type.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request.",
                details = exception.Message,
                timestamp = DateTime.UtcNow
            }
        };

        switch (exception)
        {
            case BlockchainNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;

            case InvalidBlockchainDataException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;

            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
