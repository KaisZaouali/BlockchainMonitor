using BlockchainMonitor.Domain.Interfaces;
using BlockchainMonitor.Infrastructure.Data;
using BlockchainMonitor.Infrastructure.Repositories;
using BlockchainMonitor.Infrastructure.Services;
using BlockchainMonitor.Infrastructure.Interfaces;
using BlockchainMonitor.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BlockchainMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework
        services.AddDbContext<BlockchainDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register configuration
        services.Configure<RetrySettings>(configuration.GetSection(RetrySettings.SectionName));
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        services.Configure<MetricsSettings>(configuration.GetSection(MetricsSettings.SectionName));

        // Register Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        // Register repositories
        services.AddScoped<IBlockchainRepository, BlockchainRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register external services
        services.AddHttpClient();
        services.AddSingleton<IBlockCypherService, BlockCypherService>();
        
        // Register event publisher
        services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();
        
        services.AddSingleton<IMetricsService, RedisMetricsService>();
        
        // Register cache service
        services.AddSingleton<ICacheService, CacheService>();
        
        return services;
    }

    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.Configure<MetricsSettings>(configuration.GetSection(MetricsSettings.SectionName));

        // Register Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });
        
        services.AddSingleton<IMetricsService, RedisMetricsService>();
        
        return services;
    }
} 