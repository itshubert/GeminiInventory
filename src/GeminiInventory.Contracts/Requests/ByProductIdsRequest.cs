namespace GeminiInventory.Contracts.Requests;

public sealed record ByProductIdsRequest(IEnumerable<Guid> ProductIds);