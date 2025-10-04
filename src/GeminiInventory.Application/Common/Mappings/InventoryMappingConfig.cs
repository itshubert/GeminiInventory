using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Domain.InventoryAggregate;
using GeminiInventory.Domain.InventoryAggregate.Events;
using Mapster;

namespace GeminiInventory.Application.Common.Mappings;

public sealed class InventoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Inventory, InventoryModel>()
            .Map(dest => dest, src => src);

        config.NewConfig<InventoryReservedDomainEvent, InventoryReservedIntegrationModel>()
            .Map(dest => dest.InventoryId, src => src.InventoryId.Value)
            .Map(dest => dest, src => src);
    }
}