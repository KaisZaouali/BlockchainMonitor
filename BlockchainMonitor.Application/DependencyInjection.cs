using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Services;
using BlockchainMonitor.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BlockchainMonitor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        
        // Register configuration
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        
        // Register application services
        services.AddScoped<IBlockchainService, BlockchainService>();
        services.AddScoped<ICacheService, CacheService>();
        
        return services;
    }
} 