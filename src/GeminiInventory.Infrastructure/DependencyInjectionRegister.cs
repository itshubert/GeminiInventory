using Amazon;
using Amazon.EventBridge;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;
using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Infrastructure.Messaging;
using GeminiInventory.Infrastructure.Messaging.EventProcessors;
using GeminiInventory.Infrastructure.Messaging.Events;
using GeminiInventory.Infrastructure.Persistence;
using GeminiInventory.Infrastructure.Persistence.Interceptors;
using GeminiInventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        // Log AWS configuration on startup
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<GeminiInventoryDbContext>>();
            var useLocalStack = configuration.GetValue<bool>("AWS:UseLocalStack");
            logger.LogInformation("AWS Configuration - UseLocalStack: {UseLocalStack}", useLocalStack);
            return sp;
        });

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IAmazonSQS>>();
            var useLocalStack = configuration.GetValue<bool>("AWS:UseLocalStack");

            if (useLocalStack)
            {
                var serviceUrl = configuration["AWS:LocalStack:ServiceUrl"] ?? "http://localhost:4566";
                logger.LogInformation("Configuring AmazonSQSClient for LocalStack at {ServiceUrl}", serviceUrl);
                var config = new AmazonSQSConfig { ServiceURL = serviceUrl };
                // Use AnonymousAWSCredentials for LocalStack - it doesn't validate credentials
                return new AmazonSQSClient(new AnonymousAWSCredentials(), config);
            }

            var profileName = Environment.GetEnvironmentVariable("AWS_PROFILE");
            if (!string.IsNullOrEmpty(profileName))
            {
                var credentialProfileStoreChain = new CredentialProfileStoreChain();
                if (credentialProfileStoreChain.TryGetProfile(profileName, out var profile))
                {
                    var credentials = profile.GetAWSCredentials(credentialProfileStoreChain);
                    return new AmazonSQSClient(credentials, RegionEndpoint.GetBySystemName("ap-southeast-2"));
                }
            }

            return new AmazonSQSClient(RegionEndpoint.GetBySystemName("ap-southeast-2"));
        });

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        // AWS EventBridge - Configure for LocalStack or AWS
        ConfigureEventBridge(services, configuration);
        services.AddScoped<IEventBridgePublisher, EventBridgePublisher>();

        // SQS Polling Background Service
        services.Configure<QueueSettings>(configuration.GetSection("QueueSettings"));
        services.AddMessaging<OrderSubmittedEvent, OrderSubmittedEventProcessor>(sp =>
        {
            return sp.GetRequiredService<IOptions<QueueSettings>>().Value.OrderSubmitted ?? string.Empty;
        });

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
                // Use AnonymousAWSCredentials for LocalStack - it doesn't validate credentials
                Credentials = new AnonymousAWSCredentials(),
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

    private static IServiceCollection AddMessaging<TEvent, TProcessor>(
        this IServiceCollection services,
        Func<IServiceProvider, string> queueUrlFactory)
            where TProcessor : class, IEventProcessor<TEvent>
    {


        services.AddScoped<IEventProcessor<TEvent>, TProcessor>();

        services.AddHostedService(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<SqsConsumerBackgroundService<TEvent, TProcessor>>>();
            var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var sqs = sp.GetRequiredService<IAmazonSQS>();
            var queueUrl = queueUrlFactory(sp);

            logger.LogInformation("Starting SQS consumer for queue: {QueueUrl}", queueUrl);

            return new SqsConsumerBackgroundService<TEvent, TProcessor>(
                logger,
                serviceScopeFactory,
                sqs,
                queueUrl);
        });

        return services;
    }
}
