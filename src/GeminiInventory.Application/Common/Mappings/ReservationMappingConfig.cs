using GeminiInventory.Application.Common.Models.Reservations;
using GeminiInventory.Domain.ReservationAggregate;
using Mapster;

namespace GeminiInventory.Application.Common.Mappings;

public sealed class ReservationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Reservation, ReservationModel>()
            .Map(dest => dest, src => src);
    }
}