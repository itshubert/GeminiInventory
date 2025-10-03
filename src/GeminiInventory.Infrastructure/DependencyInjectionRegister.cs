using Amazon.EventBridge;
using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Infrastructure.Messaging;
using GeminiInventory.Infrastructure.Persistence;
using GeminiInventory.Infrastructure.Persistence.Interceptors;
using GeminiInventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Infrastructure;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GeminiInventoryDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("GeminiInventoryDbContext");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        // AWS EventBridge - Configure for LocalStack or AWS
        ConfigureEventBridge(services, configuration);
        services.AddScoped<IEventBridgePublisher, EventBridgePublisher>();

        // TODO: BackgroundService to poll SQS for OrderSubmitted

        return services;
    }

    private static void ConfigureEventBridge(IServiceCollection services, IConfiguration configuration)
    {
        var useLocalStack = configuration.GetValue<bool>("AWS:UseLocalStack");

        if (useLocalStack)
        {
            // Configure for LocalStack
            var serviceUrl = configuration["AWS:LocalStack:ServiceUrl"] ?? "http://localhost:4566";

            services.AddAWSService<IAmazonEventBridge>(new Amazon.Extensions.NETCore.Setup.AWSOptions
            {
                DefaultClientConfig =
                {
                    ServiceURL = serviceUrl
                }
            });

            // Log LocalStack configuration
            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IAmazonEventBridge>>();
                logger.LogInformation("EventBridge configured for LocalStack at {ServiceUrl}", serviceUrl);
                return sp;
            });
        }
        else
        {
            // Configure for AWS
            services.AddAWSService<IAmazonEventBridge>();

            // Log AWS configuration
            services.AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IAmazonEventBridge>>();
                var region = configuration["AWS:Region"] ?? "us-east-1";
                logger.LogInformation("EventBridge configured for AWS in region {Region}", region);
                return sp;
            });
        }
    }
}
