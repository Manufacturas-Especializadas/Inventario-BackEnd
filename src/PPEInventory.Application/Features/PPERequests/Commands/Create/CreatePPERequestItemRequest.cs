namespace PPEInventory.Application.Features.PPERequests.Commands.Create;

public record CreatePPERequestItemRequest(
    int PPEProductId,
    int Quantity);