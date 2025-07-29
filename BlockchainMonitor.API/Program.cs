using BlockchainMonitor.Application;
using BlockchainMonitor.Infrastructure;
using BlockchainMonitor.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Application Services
builder.Services.AddApplicationServices();

// Add Infrastructure Services (needed for database access)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Map controllers
app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");

// Map root endpoint
app.MapGet("/", () => "BlockchainMonitor API is running!");

app.Run();
