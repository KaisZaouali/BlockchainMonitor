using BlockchainMonitor.Application;
using BlockchainMonitor.Infrastructure;
using BlockchainMonitor.API.Middleware;
using BlockchainMonitor.API.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register RabbitMQ event consumer
builder.Services.AddHostedService<BlockchainDataCreatedConsumer>();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.RejectionStatusCode = 429; // Return 429 Too Many Requests
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too Many Requests. Please try again later.", token);
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // React default
                "http://localhost:4200",  // Angular default
                "http://localhost:8080",  // Vue default
                "https://localhost:3000",
                "https://localhost:4200",
                "https://localhost:8080"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Application Services
builder.Services.AddApplicationServices(builder.Configuration);

// Add Infrastructure Services (needed for database access)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // redirects HTTP requests to HTTPS for security

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff"); // prevents MIME type sniffing
    context.Response.Headers.Append("X-Frame-Options", "DENY"); // prevents clickjacking
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block"); // prevents XSS
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin"); // prevents referrer leakage
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()"); // prevents browser fingerprinting
    await next();
});

// Use CORS
app.UseCors("AllowSpecificOrigins");

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use rate limiting
app.UseRateLimiter();

// Map controllers
app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");

// Map root endpoint
app.MapGet("/", () => "BlockchainMonitor API is running!");

app.Run();

// Make Program accessible for testing
public partial class Program { }
