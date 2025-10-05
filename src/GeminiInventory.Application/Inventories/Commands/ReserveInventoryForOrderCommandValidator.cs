using FluentValidation;

namespace GeminiInventory.Application.Inventories.Commands;

public sealed class ReserveInventoryForOrderCommandValidator : AbstractValidator<ReserveInventoryForOrderCommand>
{
    public ReserveInventoryForOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotNull().WithMessage("Order ID is required.")
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.Items)
            .NotNull()
            .NotEmpty().WithMessage("At least one inventory item is required.")
            .Must(items => items.Any()).WithMessage("At least one inventory item is required.")
            .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count()).WithMessage("Duplicate Product IDs are not allowed.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId)
                .NotNull().WithMessage("Product ID is required.")
                .NotEmpty().WithMessage("Product ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Product ID cannot be an empty GUID.");

            items.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}