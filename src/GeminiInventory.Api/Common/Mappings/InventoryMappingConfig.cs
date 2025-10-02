using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Contracts.Responses;
using Mapster;

namespace GeminiInventory.Api.Common.Mappings;

public sealed class InventoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<InventoryModel, InventoryResponse>()
            .Map(dest => dest, src => src);
    }
}