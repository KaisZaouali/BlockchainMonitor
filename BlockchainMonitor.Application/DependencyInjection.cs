using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockchainMonitor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IBlockchainService, BlockchainService>();
        
        return services;
    }
} 