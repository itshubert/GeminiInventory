using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Domain.InventoryAggregate;
using Mapster;

namespace GeminiInventory.Application.Common.Mappings;

public sealed class InventoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Inventory, InventoryModel>()
            .Map(dest => dest, src => src);
    }
}