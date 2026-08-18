using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Commands.Create;

public record CreatePPERequestCommand(
    string EmployeeNumber,
    int WarehouseId,
    int RequestReasonId,
    string? Notes,
    IReadOnlyCollection<CreatePPERequestItemRequest> Items)
    : IRequest<CreatePPERequestResultDto>;