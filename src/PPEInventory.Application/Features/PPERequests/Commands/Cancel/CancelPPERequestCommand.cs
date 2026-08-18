using MediatR;
using PPEInventory.Application.Features.PPERequests.Cancellation;

namespace PPEInventory.Application.Features.PPERequests.Commands.Cancel;

public record CancelPPERequestCommand(
    string Folio,
    string CancellationReason)
    : IRequest<CancelPPERequestResultDto>;