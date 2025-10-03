using ErrorOr;
using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Domain.Common.DomainErrors;
using MapsterMapper;
using MediatR;

namespace GeminiInventory.Application.Inventories.Commands;

public sealed record UpdateInventoryStockLevelCommand(Guid ProductId, int NewStock) : IRequest<ErrorOr<InventoryModel>>;

public sealed class UpdateInventoryStockLevelCommandHandler : IRequestHandler<UpdateInventoryStockLevelCommand, ErrorOr<InventoryModel>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public UpdateInventoryStockLevelCommandHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<InventoryModel>> Handle(UpdateInventoryStockLevelCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdForUpdateAsync(request.ProductId, cancellationToken);
        if (inventory is null)
        {
            return Errors.Inventory.InvalidProductId;
        }

        inventory.UpdateStockLevel(request.NewStock);

        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<InventoryModel>(inventory);
    }
}