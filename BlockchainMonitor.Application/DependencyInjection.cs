using BlockchainMonitor.Application.Interfaces;
using BlockchainMonitor.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BlockchainMonitor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        // Register application services
        services.AddScoped<IBlockchainService, BlockchainService>();

        return services;
    }
}
