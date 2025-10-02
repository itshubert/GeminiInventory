using ErrorOr;
using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Domain.Common.DomainErrors;
using MapsterMapper;
using MediatR;

namespace GeminiInventory.Application.Inventories.Queries;

public sealed record GetInventoryByProductQuery(Guid ProductId) : IRequest<ErrorOr<InventoryModel?>>;

public sealed class GetInventoryByProductQueryHandler : IRequestHandler<GetInventoryByProductQuery, ErrorOr<InventoryModel?>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoryByProductQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<InventoryModel?>> Handle(GetInventoryByProductQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId, cancellationToken);

        if (inventory is null)
        {
            return Errors.Inventory.InvalidProductId;
        }

        var inventoryModel = _mapper.Map<InventoryModel>(inventory);
        return inventoryModel;
    }
}