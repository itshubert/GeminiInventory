using ErrorOr;

namespace GeminiInventory.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class Inventory
    {
        public static Error InvalidProductId => Error.Validation(
            code: "Inventory.InvalidProductId",
            description: "The provided product ID is invalid or does not exist.");
    }
}
