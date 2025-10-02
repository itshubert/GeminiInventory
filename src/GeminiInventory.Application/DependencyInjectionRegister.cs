using GeminiInventory.Application.Common.Behaviors;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GeminiInventory.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        services.AddMappings();

        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(AppDomain.CurrentDomain.GetAssemblies());

        services.AddSingleton(config);
        services.AddMapster();

        return services;
    }
}
