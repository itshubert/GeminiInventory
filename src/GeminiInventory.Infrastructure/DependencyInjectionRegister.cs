using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Infrastructure.Persistence;
using GeminiInventory.Infrastructure.Persistence.Interceptors;
using GeminiInventory.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // TODO: BackgroundService to poll SQS for OrderSubmitted

        return services;
    }
}
