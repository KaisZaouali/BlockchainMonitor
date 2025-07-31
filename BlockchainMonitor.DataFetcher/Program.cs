using BlockchainMonitor.Application;
using BlockchainMonitor.Infrastructure;
using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.DataFetcher.Services;
using BlockchainMonitor.DataFetcher.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Add Application Services
builder.Services.AddApplicationServices(builder.Configuration);

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add DataFetcher Services
builder.Services.AddScoped<IDataFetchingService, DataFetchingService>();
builder.Services.Configure<DataFetchingSettings>(
    builder.Configuration.GetSection(DataFetchingSettings.SectionName));

// Add background service
builder.Services.AddHostedService<BlockchainDataFetchingBackgroundService>();

var host = builder.Build();

Console.WriteLine("Blockchain Data Fetcher Service Starting...");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine("Press Ctrl+C to stop the service.");

await host.RunAsync();
