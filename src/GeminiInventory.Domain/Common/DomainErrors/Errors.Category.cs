using ErrorOr;

namespace GeminiInventory.Domain.Common.DomainErrors;

public static partial class Errors
{
    public static class Category
    {
        public static Error InvalidCategoryId => Error.Validation(
            code: "Category.InvalidId",
            description: "The provided category ID is invalid or does not exist.");
    }
}
