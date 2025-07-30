using BlockchainMonitor.Application;
using BlockchainMonitor.Infrastructure;
using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.DataFetcher.Services;
using BlockchainMonitor.DataFetcher.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Set the base path to the project directory for configuration files
var projectDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);
if (projectDirectory != null)
{
    builder.Configuration.SetBasePath(projectDirectory);
}

// Explicitly add JSON configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add Application Services
builder.Services.AddApplicationServices(builder.Configuration);

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add DataFetcher Services
builder.Services.AddScoped<IDataFetchingService, DataFetchingService>();
builder.Services.Configure<BlockchainDataFetchingOptions>(
    builder.Configuration.GetSection("BlockchainDataFetching"));
builder.Services.Configure<DataFetchingSettings>(
    builder.Configuration.GetSection(DataFetchingSettings.SectionName));

// Add background service
builder.Services.AddHostedService<BlockchainDataFetchingBackgroundService>();

var host = builder.Build();

Console.WriteLine("Blockchain Data Fetcher Service Starting...");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine("Press Ctrl+C to stop the service.");

await host.RunAsync();
