using FluentValidation;

namespace GeminiInventory.Application.Inventories.Commands;

public sealed class UpdateInventoryStockLevelCommandValidator : AbstractValidator<UpdateInventoryStockLevelCommand>
{
    public UpdateInventoryStockLevelCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId must not be empty.")
            .NotEqual(Guid.Empty).WithMessage("ProductId must not be an empty GUID.");

        RuleFor(x => x.NewStock)
            .GreaterThanOrEqualTo(0).WithMessage("NewStock must be greater than or equal to 0.");
    }
}