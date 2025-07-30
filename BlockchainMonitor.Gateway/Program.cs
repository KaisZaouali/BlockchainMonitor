using Yarp.ReverseProxy.Configuration;
using BlockchainMonitor.Gateway.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100, // Requests per window
                Window = TimeSpan.FromMinutes(1) // Window size
            }));
});

var app = builder.Build();

// Configure the application to listen on port 80 in containers
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseGatewayMiddleware();

app.MapControllers();

// Map YARP reverse proxy
app.MapReverseProxy();

app.Run();

// Make Program accessible for testing
public partial class Program { }
